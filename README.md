# Proyecto Final Ingeniería de Software

Proyecto final desarrollado para el curso de Ingeniería de Software.  
Consiste en una API REST para la gestión de una banca en línea, desarrollada con ASP.NET Core, Entity Framework Core y SQL Server.

## Descripción

El sistema permite administrar usuarios, roles, cuentas bancarias, beneficiarios, transferencias, solicitudes de préstamo, préstamos, pagos y reportes generales.

El backend expone endpoints REST que permiten realizar operaciones CRUD sobre las principales entidades del sistema y consultar información resumida para reportes.

## Tecnologías Utilizadas

- ASP.NET Core 8
- C#
- Entity Framework Core 8
- SQL Server
- Swagger / OpenAPI
- Visual Studio
- Azure App Service

## Módulos Principales
- Usuarios
- Roles
- Cuentas bancarias
- Beneficiarios
- Transferencias
- Solicitudes de préstamo
- Préstamos
- Pagos de préstamo
- Movimientos de cuenta
- Reportes

## Estructura del Proyecto

```txt
ProyectoFinalIngSoftware/
├── Backend/
│   ├── IngSoftwareBackend.sln
│   └── IngSoftwareBackend/
│       ├── Controllers/
│       ├── Data/
│       ├── DTOs/
│       ├── Models/
│       ├── Program.cs
│       ├── appsettings.json
│       └── IngSoftwareBackend.csproj
├── DB/
│   └── Queries.sql
└── README.md
