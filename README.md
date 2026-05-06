---
title: "StarterApp readme"
parent: StarterApp
grand_parent: C# practice
nav_order: 5
mermaid: true
---

# RentalApp — Library of Things

A peer-to-peer rental marketplace built with .NET MAUI, extending the StarterApp for SET09102.
Community members can list items for rent, discover items and request rentals.

The app provides all StarterApp features plus:
* REST API integration with JWT authentication
* Item listing and browsing
* Rental request workflow
* Incoming and outgoing rental viewing

This version connects to a shared REST API backend.

To fully understand how it works, you should follow an appropriate set of tutorials such as 
[this one](https://edinburgh-napier.github.io/SET09102/tutorials/csharp/) which covers all of the main
concepts and techniques used here. However, if you want to jump straight in and work out any problems
as you go along, that will also work. The code uses structured comments for use with the 
[Doxygen](https://www.doxygen.nl/) documentation generator tool. 

You can use any development environment with this project including

* [Rider](https://www.jetbrains.com/rider/)
* [Visual Studio](https://visualstudio.microsoft.com/)
* [Visual Studio Code](https://code.visualstudio.com/)

The instructions assume you will be using VSCode since that is a lowest-common-denominator choice.

## Compatibility

This app is built using the following tool versions.

| Name                                                                                      | Version     |
|-------------------------------------------------------------------------------------------|-------------|
| [.NET](https://dotnet.microsoft.com/en-us/)                                               | 8.0 / 9.0   |
| [PostgreSQL Docker image](https://hub.docker.com/_/postgres)                              | 16          |
| [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui)                                 | 10.0        |


## Getting started

### Prerequisites

Before using this app, ensure you have:

1. **.NET SDK 8.0** or later installed
2. **Docker** installed and running
3. **PostgreSQL container** running (see [dev-environment tutorial](https://edinburgh-napier.github.io/SET09102/tutorials/csharp/dev-environment/))
4. **Android emulator** running

### Configuration

1. Copy `StarterApp.Database/appsettings.json.template` to `StarterApp.Database/appsettings.json`
2. Update the connection string with your PostgreSQL credentials:
   ```json
   {
     "ConnectionStrings": {
       "DevelopmentConnection": "Host=localhost;Username=student_user;Password=password123;Database=starterapp"
     }
   }
   ```

### Initial Setup

1. Navigate to the Migrations project and create the initial migration:
   ```bash
   cd StarterApp.Migrations
   dotnet ef migrations add InitialCreate
   ```

2. Apply the migration to create the database:
   ```bash
   dotnet ef database update
   ```

3. Build and run the application:
   ```bash
   cd ../StarterApp
   dotnet clean
   dotnet build RentalApp/RentalApp.csproj -f net10.0-android -c Debug
   ```

 4. Install on emulator:
   ```cmd
   adb uninstall com.companyname.starterapp
   adb install RentalApp\bin\Debug\net10.0-android\com.companyname.starterapp-Signed.apk
   adb shell monkey -p com.companyname.starterapp 1
   ```  

### Running Tests

```bash
dotnet test RentalApp.Test/RentalApp.Test.csproj --verbosity normal
```

### Tutorial

For a comprehensive guide on using this app and understanding its architecture, see the
[MAUI + MVVM + Database Tutorial](https://edinburgh-napier.github.io/SET09102/tutorials/csharp/maui-mvvm-database/).
