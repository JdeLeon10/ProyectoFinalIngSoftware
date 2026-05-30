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
Módulos Principales
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
Base de Datos
El script de creación de base de datos se encuentra en:
DB/Queries.sql
Este script crea la base de datos:
BancaEnLineaDB
También crea las tablas principales del sistema:
- rol
- usuario
- cuenta
- beneficiario
- transferencia
- movimiento_cuenta
- solicitud_prestamo
- prestamo
- cuota_prestamo
- pago_prestamo
Además incluye datos iniciales para pruebas, como roles, usuarios, cuentas y movimientos iniciales.
Configuración de la Cadena de Conexión
La cadena de conexión se configura en:
Backend/IngSoftwareBackend/appsettings.json
Ejemplo local:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BancaEnLineaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Para despliegue en Azure, se debe configurar una cadena de conexión válida hacia una base de datos SQL Server accesible desde Azure.
Ejecución Local
Ubicarse en la carpeta del proyecto backend:
cd Backend/IngSoftwareBackend
Restaurar dependencias:
dotnet restore
Ejecutar el proyecto:
dotnet run
La API se ejecutará localmente y podrá consumirse desde la URL mostrada en consola.
Swagger
El proyecto tiene Swagger configurado para ambiente de desarrollo.
Cuando el proyecto se ejecuta en modo Development, se puede acceder a la documentación interactiva en:
/swagger
Endpoints Principales
Usuarios
GET /allusers
GET /api/Users/user/{id}
POST /createuser
PUT /api/Users/edituser/{id}
PUT /api/Users/toggleuser/{id}
DELETE /api/Users/deleteuser/{id}
Roles
GET /allroles
GET /api/Roles/rol/{id}
POST /createrol
PUT /api/Roles/editrol/{id}
PUT /api/Roles/togglerol/{id}
DELETE /api/Roles/deleterol/{id}
Cuentas
GET /allcuentas
GET /api/Cuentas/cuenta/{id}
POST /createcuenta
PUT /api/Cuentas/editcuenta/{id}
PUT /api/Cuentas/togglecuenta/{id}
DELETE /api/Cuentas/deletecuenta/{id}
Beneficiarios
GET /allbeneficiarios
GET /api/Beneficiarios/beneficiario/{id}
POST /createbeneficiario
PUT /api/Beneficiarios/editbeneficiario/{id}
PUT /api/Beneficiarios/togglebeneficiario/{id}
DELETE /api/Beneficiarios/deletebeneficiario/{id}
Transferencias
GET /alltransferencias
GET /api/Transferencias/transferencia/{id}
POST /createtransferencia
PUT /api/Transferencias/edittransferencia/{id}
PUT /api/Transferencias/toggletransferencia/{id}
DELETE /api/Transferencias/deletetransferencia/{id}
Solicitudes de Préstamo
GET /allsolicitudprestamos
GET /api/SolicitudPrestamos/solicitudprestamo/{id}
POST /createsolicitudprestamo
PUT /api/SolicitudPrestamos/editsolicitudprestamo/{id}
PUT /api/SolicitudPrestamos/togglesolicitudprestamo/{id}
DELETE /api/SolicitudPrestamos/deletesolicitudprestamo/{id}
Préstamos
GET /allprestamos
GET /api/Prestamos/prestamo/{id}
POST /createprestamo
PUT /api/Prestamos/editprestamo/{id}
PUT /api/Prestamos/toggleprestamo/{id}
DELETE /api/Prestamos/deleteprestamo/{id}
Pagos
GET /allpagos
GET /api/Pagos/pago/{id}
POST /createpago
PUT /api/Pagos/editpago/{id}
PUT /api/Pagos/togglepago/{id}
DELETE /api/Pagos/deletepago/{id}
Reportes
GET /allreportes
GET /api/Reportes/reporte/{id}
Valores disponibles para reportes individuales:
usuarios
cuentas
transferencias
solicitudes
prestamos
pagos
Publicación en Azure App Service
Para publicar el backend en Azure App Service se debe usar el proyecto:
Backend/IngSoftwareBackend/IngSoftwareBackend.csproj
El App Service debe estar configurado con:
Stack: .NET
Version: .NET 8
También se debe configurar la cadena de conexión en Azure, ya que la conexión local con localhost no funciona en producción.
Notas Importantes
- El proyecto usa Entity Framework Core con SQL Server.
- Swagger está habilitado únicamente en ambiente de desarrollo.
- La ruta raíz / no tiene un endpoint definido actualmente.
- Los endpoints principales utilizan rutas absolutas como /allusers, /allcuentas y /alltransferencias.
- Para producción se recomienda configurar correctamente la cadena de conexión desde Azure App Service.
- El proyecto no incluye autenticación todavía.
- El controlador WeatherForecast corresponde al ejemplo base de ASP.NET Core.
Autor
Proyecto Final UMG  
Ingeniería en Sistemas  
Plan Sábado  
9no Semestre
