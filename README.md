# Webmail
Webmail project for SI .NET

1. How to run:
```shell
docker-compose -f docker-compose.ci.build.yml up --build
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --build
```
Application can be accesed via http://localhost

2. How to stop:
```shell
docker-compose down
