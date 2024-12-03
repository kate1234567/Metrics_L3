# IO.Swagger - ASP.NET Core 2.0 Server

Это пример сервиса управления фильмами, реализованного на основе спецификации OpenAPI 3.0. 

## Run

Linux/OS X:

```
sh build.sh
```

Windows:

```
build.bat
```

## Run in Docker

```
cd src/IO.Swagger
docker build -t io.swagger .
docker run -p 5000:5000 io.swagger
```
