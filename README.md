# Financial-Services-API
Proyecto desarrollado como parte de una prueba técnica en .NET, que implementa una API REST para la gestión de clientes, préstamos y cálculo de intereses. Incluye estructura por capas (Entities, Repositories, Services, Controllers) y pruebas unitarias para las entidades y servicios principales. 

Incluye pruebas unitarias desarrolladas con xUnit y una base de datos InMemory para validación de lógica.

🚀 Requisitos previos
- .NET 8 SDK
- Visual Studio 2022 / Visual Studio Code
- Git

1. Clonar el repositorio:
git clone https://github.com/MaradiagaDev/Financial-Services-API.git

2.Entrar al proyecto:
cd FinancialServices/FinancialServices.Api

3.Ejecutar la API:
dotnet run

4.La API se ejecutará por defecto en:
https://localhost:5001
http://localhost:5000

5. Para probar los endpoints en:
Swagger: (https://localhost:5001/swagger/index.html)

## 🚀 Features principales

- Registrar y consultar clientes
- Crear cuentas bancarias para clientes
- Ejecutar transacciones (depósito / retiro)
- Consultar saldo y historial de movimientos
- Simulación de aplicación de intereses (método en service)
- Pruebas unitarias para Entities y Services (xUnit + InMemory DB)
- Swagger para explorar la API


