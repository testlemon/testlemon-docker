using System.Diagnostics;
using CommandLine;
using console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Testlemon.Core;
using Testlemon.Core.Extensions;
using Testlemon.Core.Models;
using Testlemon.Core.Output;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(Run, _ => Task.FromResult(1));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    static async Task<int> Run(Options opts)
    {
        ShowWelcomeMessage();

        // Setup the DI container
        var sp = new ServiceCollection()
                .AddSingleton<IConfiguration>(sp =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddEnvironmentVariables()
                        .AddJsonFile("config.json", true)
                        .Build();

                    return configuration;
                })
                .AddSingleton<Testlemon.OllamaClient.OllamaClient>(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var endpoint = opts.OllamaEndpoint ?? configuration[Testlemon.OllamaClient.OllamaClient.OLLAMA_ENDPOINT] ?? string.Empty;
                    var timeout = opts.OllamaTimeout;
                    return new Testlemon.OllamaClient.OllamaClient(endpoint, (uint)timeout);
                })
                .AddSingleton<Testlemon.OpenAIClient.OpenAIClient>(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var endpoint = opts.OpenAIEndpoint ?? configuration[Testlemon.OpenAIClient.OpenAIClient.OPEN_AI_ENDPOINT] ?? string.Empty;
                    var apiKey = opts.OpenAIApiKey ?? configuration[Testlemon.OpenAIClient.OpenAIClient.OPEN_AI_APIKEY] ?? string.Empty;
                    return new Testlemon.OpenAIClient.OpenAIClient(endpoint, apiKey, 1000);
                })
                .AddSingleton<Testlemon.LLMClient.LlmClient>()
                .AddSingleton<DataProcessor>()
                .AddSingleton<ValidationProcessor>()
                .AddAutoDiscoveredValidatorsFromAssembly(Path.Combine(AppContext.BaseDirectory, "testlemon.dll"))
                .AddSingleton<CollectionRunner>(sp =>
                {
                    var dataProcessor = sp.GetRequiredService<DataProcessor>();
                    var validationProcessor = sp.GetRequiredService<ValidationProcessor>();
                    return new CollectionRunner(dataProcessor, validationProcessor);
                })
                .BuildServiceProvider();

        var runner = sp.GetRequiredService<CollectionRunner>();

        while (true)
        {
            try
            {
                var exitCode = await RunAndReturnExitCodeAsync(runner, opts);

                if (opts.Interval == 0)
                {
                    return exitCode;
                }
                else
                {
                    Console.WriteLine($"Last exit code: {exitCode}.");
                    await Task.Delay((int)opts.Interval);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}.");
                return 1; // Error code
            }
        }
    }

    static void ShowWelcomeMessage()
    {
        Console.WriteLine("Welcome to Testlemon!\nCreated by https://itbusina.com.\nContact contact@testlemon.com in case any questions.");
    }

    static async Task<int> RunAndReturnExitCodeAsync(CollectionRunner runner, Options opts)
    {
        // Initialize collection runner and execute requests. Measure the duration.
        var stopWatch = Stopwatch.StartNew();
        var collectionResults = await runner.ExecuteAsync(opts.Collections, opts.GetParsedHeaders(), opts.GetParsedVariables(), opts.GetParsedSecrets(), opts.Tags, opts.Parallel, (uint)opts.Repeats, (uint)opts.Delay, opts.FollowRedirect);
        stopWatch.Stop();

        ShowResultsSummary(collectionResults, stopWatch);

        // print results to output.
        if (!string.IsNullOrWhiteSpace(opts.Output))
        {
            foreach (var result in collectionResults)
            {
                await SaveCollectionReportAsync(result, opts);
            }
        }

        // show results on console
        if (opts.Verbose == true)
        {
            foreach (var result in collectionResults)
            {
                var jsonResult = result.ToJsonOutput();

                foreach (var secret in opts.Secrets)
                {
                    var secretValue = secret.Split('=')[1];
                    jsonResult = jsonResult.Replace(secretValue, "*****");
                }

                Console.WriteLine(jsonResult);
            }
        }

        // Return exit code
        if (collectionResults.Any(x => !x.IsValid))
            return 1;

        return 0;
    }

    static void ShowResultsSummary(IEnumerable<CollectionResult> collectionResults, Stopwatch stopWatch)
    {
        var allRequests = collectionResults.SelectMany(x => x.TestsResults).ToList().Where(x => x != null).ToList(); ;

        // Show the execution summary
        var successful = allRequests.Count(x => x is { IsValid: true });
        var failed = allRequests.Count(x => x is { IsValid: false });
        
        Console.WriteLine($"\nTotal requests: {allRequests.Count}\nSuccessful: {successful}\nFailed: {failed}\nTotal Duration: {stopWatch.Elapsed}");
    }

    static async Task SaveCollectionReportAsync(CollectionResult collectionResult, Options opts)
    {
        string output;
        switch (opts.OutputFormat.ToLowerInvariant())
        {
            case "xray":
                {
                    output = collectionResult.ToXRayOutput();
                    break;
                }
            case "json":
            default:
                {
                    output = collectionResult.ToJsonOutput();
                    break;
                }
        }

        //save output to file
        var fileName = opts.Output;
        await File.WriteAllTextAsync($"./{fileName}", output);

        var fileInfo = new FileInfo($"./{fileName}");
        Console.WriteLine($"Results are saved to {fileInfo.FullName} file.");
    }
}