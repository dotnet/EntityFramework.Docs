---
title: Design-time services - EF Core
author: bricelam
ms.author: bricelam
ms.date: 10/26/2017
---
Design-time services
====================
Some services used by the tools are only used at design time. These services are managed separately from EF Core's
runtime services to prevent them from being deployed with your app. To override one of these services (for example the
service to generate migration files), add an implementation of `IDesignTimeServices` to your startup project.

``` csharp
class MyDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
        => services.AddSingleton<IMigrationsCodeGenerator, MyMigrationsCodeGenerator>()
}
```
