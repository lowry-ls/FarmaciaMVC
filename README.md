# FarmaciaMVC

Aplicacion web ASP.NET Core MVC con PostgreSQL mediante Npgsql.

## Requisitos

- .NET 10 SDK
- PostgreSQL accesible desde la aplicacion

## Configuracion local

La cadena de conexion no se guarda en el repositorio. Configurala con User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=farmacia_db;Username=TU_USUARIO;Password=TU_CONTRASENA"
```

Tambien puedes establecer la variable de entorno `ConnectionStrings__DefaultConnection`.

## Ejecutar

```powershell
dotnet restore
dotnet run
```

El endpoint de diagnostico `GET /db-check` solo se registra en el entorno `Development`. Devuelve un error generico si la conexion no esta configurada o no se puede abrir; el detalle se registra en el servidor.

## Estado actual

El proyecto contiene la estructura inicial de MVC y el endpoint de diagnostico de PostgreSQL. Aun no incluye funcionalidades de inventario, ventas ni modelos de farmacia.
