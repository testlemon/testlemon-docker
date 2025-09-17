# Use the official .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Use the secret only for restore (doesn't get saved in layers)
RUN --mount=type=secret,id=nuget_token \
    GITHUB_TOKEN=$(cat /run/secrets/nuget_token) && \
    dotnet nuget add source "https://nuget.pkg.github.com/testlemon/index.json" \
        --name github \
        --username testlemon \
        --password $GITHUB_TOKEN \
        --store-password-in-clear-text && \
    dotnet restore

# Copy project files
COPY ./src/ ./src

# Publish app to out folder
WORKDIR /src
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /src/out .

# Create a directory for the application and set permissions
#RUN chown -R testlemon:testlemon /app

# Switch to the non-root user
#USER testlemon

# Entry point when the container starts
ENTRYPOINT ["dotnet", "console.dll"]
