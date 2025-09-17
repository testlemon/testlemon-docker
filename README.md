# Oficial Testlemon Docker Image
[![Docker Pulls](https://img.shields.io/docker/pulls/itbusina/testlemon)](https://hub.docker.com/r/itbusina/testlemon)
[![Docker Stars](https://img.shields.io/docker/stars/itbusina/testlemon)](https://hub.docker.com/r/itbusina/testlemon)

Testlemon is a tool for easy and quick web api testing and monitoring.

# Documentation
Vitis [docs.testlemon.com](https://docs.testlemon.com) for complete documentation.

# Quick start

## Run tests from OpenAPI specification
```bash
docker run --rm itbusina/testlemon -c https://api.mockrest.com/openapi/v1.json
```

## Run tests from file

Create a file with tests ```collection.yaml```

```text
tests:
- url: https://dummyjson.com/users
- url: https://dummyjson.com/comments
- url: https://dummyjson.com/products
```

Run the tests:

```bash
docker run --rm itbusina/testlemon -c "$(<.collection.yaml)"
```

## Run tests from url

Create a file and make it available by url.

```shell
docker run --rm itbusina/testlemon:latest -c https://raw.githubusercontent.com/itbusina/testlemon-docs/refs/heads/main/examples/quick-start.yaml
```

## Run tests from multiple test collections

Create multiple test collections.

```shell
docker run --rm itbusina/testlemon:latest -c "$(<collection-1.yaml)" "$(<collection-2.yaml)"
```

## Run test collection with variables

Create test collection with variable placeholders.

```text
tests:
- url: ${{ vars.host }}/users
- url: ${{ vars.host }}/comments
- url: ${{ vars.host }}/products
```

```shell
docker run --rm itbusina/testlemon:latest -c "$(<collection.yaml)" --variables host=https://dummyjson.com
```

## Run test collection with secrets

Create test collection with variable placeholders.

```text
tests:
- url: ${{ vars.host }}/users
- url: ${{ vars.host }}/comments
- url: ${{ vars.host }}/products
```

```shell
docker run --rm itbusina/testlemon:latest -c "$(<collection.yaml)" --secrets host=https://dummyjson.com
```