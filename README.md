#Guía de Configuración 

Requisitos Previos

    Git

    SDK de .NET 8

    SQLite3

        

    Herramientas de Entity Framework Core CLI ejecutar

        dotnet tool install --global dotnet-ef


1. Clonar el Repositorio

Abre tu terminal (o Git Bash) y ejecuta el siguiente comando para clonar el repositorio en tu máquina local:


git clone https://github.com/OrtizHenoc/ApiRepo.git


2. Navegar al Directorio del Proyecto

Una vez clonado, navega al directorio de tu proyecto de la API bancaria:


cd ApiRepo/ApiBanck


3. Restaurar Dependencias

Antes de construir el proyecto, restaura todas las dependencias de NuGet:

dotnet restore

4. Actualizar la Base de Datos (Aplicar Migraciones)

dotnet ef database update


5. Ejecutar el Proyecto

Una vez que las dependencias estén restauradas y las migraciones aplicadas, puedes ejecutar la API:

dotnet run

Esto iniciará tu servidor de API. Por defecto, .NET Core suele iniciar en https://localhost:70XX y http://localhost:5XXX (donde XX son números de puerto). La terminal te indicará las URLs exactas.

7. Acceder a la API

Una vez que la API esté en ejecución, puedes acceder a ella a través de tu navegador.