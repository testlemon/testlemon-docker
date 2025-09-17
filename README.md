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

Output
```text
Welcome to Testlemon!
Created by https://itbusina.com.
Contact contact@testlemon.com in case any questions.
Testlemon is running.
Running the collection 'd6b00018-dddd-40c2-b66f-eb1aaaba86ad' 1 time(s).
POST     https://api.mockrest.com/users => OK in 00:00:00.5944665
POST     https://api.mockrest.com/products => OK in 00:00:00.6435270
DELETE   https://api.mockrest.com/products => OK in 00:00:00.7964084
POST     https://api.mockrest.com/custom => Created in 00:00:00.8279779
DELETE   https://api.mockrest.com/comments => OK in 00:00:00.9653697
DELETE   https://api.mockrest.com/users => OK in 00:00:01.0035434
POST     https://api.mockrest.com/comments => OK in 00:00:01.0071714
GET      https://api.mockrest.com/products => OK in 00:00:01.0661048
GET      https://api.mockrest.com/users => OK in 00:00:01.1363180
GET      https://api.mockrest.com/comments => OK in 00:00:01.1407234

Total requests: 10
Successful: 10
Failed: 0
Total Duration: 00:00:02.1043505
```

## Run tests from file

Create a file with tests ```collection.yaml```

```text
tests:
- url: https://api.mockrest.com/users
- url: https://api.mockrest.com/products
- url: https://api.mockrest.com/comments
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
docker run --rm itbusina/testlemon:latest -c "$(<collection.yaml)" --variables host=https://api.mockrest.com
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
docker run --rm itbusina/testlemon:latest -c "$(<collection.yaml)" --secrets host=https://api.mockrest.com
```