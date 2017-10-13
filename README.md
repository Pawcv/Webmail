# Webmail
Webmail project for SI .NET

1. How to run:
```shell
sudo docker-compose -f docker-compose.ci.build.yml up
sudo docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```
Application can be accesed on http://localhost
2. How to stop:
```shell
sudo docker-compose down
