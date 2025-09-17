# Oficial Testlemon Docker Image
[![Docker Pulls](https://img.shields.io/docker/pulls/itbusina/testlemon)](https://hub.docker.com/r/itbusina/testlemon)
[![Docker Stars](https://img.shields.io/docker/stars/itbusina/testlemon)](https://hub.docker.com/r/itbusina/testlemon)

No-Code APIs testing tool with JSON configuration.

# Documentation
[Docker image](https://hub.docker.com/repository/docker/itbusina/testlemon/general)

# Quick start
Minimal steps to test you API.

## Create a collection.json file with API requests to test.
```json
{
    "requests": [
      {
        "url": "https://dummyjson.com/products"
      }
    ]
}
```

## Run the docker image
```shell
docker run itbusina/testlemon:latest -c <collection>
```
