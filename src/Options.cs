using CommandLine;

namespace console
{
    public class Options
    {
        public Options()
        {
            Secrets = [];
            Variables = [];
            Headers = [];
            Tags = [];
        }


        [Option('c', "collections", Required = true, HelpText = "The reference to collection with requests. It can be a plan text, a path to file(s), directory(ies) or URL resource.")]
        public required IEnumerable<string> Collections { get; set; }

        [Option('h', "headers", Separator = ',', HelpText = "Http headers for URL resource.")]
        public IEnumerable<string> Headers { get; set; }

        [Option('v', "variables", Separator = ',', HelpText = "Variables which can be used in collection(s). Example -v host=https://superserver.prod.com")]
        public IEnumerable<string> Variables { get; set; }

        [Option('s', "secrets", Separator = ',', HelpText = "Secrets which can be used in collection(s). Example -s password=DJ876d%DSA44*^")]
        public IEnumerable<string> Secrets { get; set; }

        [Option('t', "tags", Separator = ',', HelpText = "Tags to filter the collection(s) requests.")]
        public IEnumerable<string> Tags { get; set; }

        [Option('p', "parallel", Default = true, HelpText = "Execute requests in parallel.")]
        public bool Parallel { get; set; }

        [Option('r', "repeats", Default = 1, HelpText = "Number of iterations.")]
        public int Repeats { get; set; }

        [Option('d', "delay", Default = 0, HelpText = "Delay between iterations in milliseconds.")]
        public int Delay { get; set; }

        [Option("followRedirect", Default = true, HelpText = "Follow redirects.")]
        public bool FollowRedirect { get; set; }

        [Option('i', "interval", Default = 0, HelpText = "Interval for continious looping in milliseconds.")]
        public int Interval { get; set; }

        [Option('o', "output", HelpText = "File path to save output results.")]
        public string Output { get; set; }

        [Option('f', "output-format", Default = "json", HelpText = "The output format. Ex: json (default), xray.")]
        public string OutputFormat { get; set; }

        [Option("verbose", Default = false, HelpText = "Show results in console.")]
        public bool Verbose { get; set; }

        [Option("openai-apikey", HelpText = "OpenAI API key.")]
        public string OpenAIApiKey { get; set; } = string.Empty;

        [Option("openai-endpoint", Default = "https://api.openai.com", HelpText = "OpenAI endpoint.")]
        public string OpenAIEndpoint { get; set; } = string.Empty;

        [Option("ollama-endpoint", Default = "http://localhost:11434", HelpText = "Ollama endpoint.")]
        public string OllamaEndpoint { get; set; } = string.Empty;

        [Option("ollama-timeout", Default = 300, HelpText = "Ollama endpoint timeout.")]
        public int OllamaTimeout { get; set; }

        public Dictionary<string, string> GetParsedVariables()
        {
            return GetParsedKeyValuePairs(Variables, '=');
        }

        public Dictionary<string, string> GetParsedSecrets()
        {
            return GetParsedKeyValuePairs(Secrets, '=');
        }

        public Dictionary<string, string> GetParsedHeaders()
        {
            return GetParsedKeyValuePairs(Headers, ':');
        }

        private static Dictionary<string, string> GetParsedKeyValuePairs(IEnumerable<string> values, char keyValueSeparator)
        {
            return values
                .Select(x => x.Split(keyValueSeparator))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0].Trim(), x => x[1].Trim());
        }
    }
}