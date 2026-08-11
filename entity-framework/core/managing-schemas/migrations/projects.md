---
title: Using a Separate Migrations Project - EF Core
description: Using a separate migration project for managing database schemas with Entity Framework Core
author: SamMonoRT
ms.date: 08/05/2026
uid: core/managing-schemas/migrations/projects
---

# Using a Separate Migrations Project

You can store migrations in a different project from the one containing your `DbContext`. This is recommended when the application project is platform-specific, such as WinUI, .NET MAUI, Blazor WebAssembly, or Azure Functions, or when it targets a specific runtime identifier (RID). It can also be used to maintain more than one set of migrations.

> [!TIP]
> You can view this article's [sample on GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Schemas/ThreeProjectMigrations).

## Project layout

The sample uses three projects:

| Project | Responsibility | References |
| --- | --- | --- |
| `WebApplication1.Data` | Owns the `DbContext` and entity types | EF Core provider |
| `WebApplication1.Migrations` | Owns migrations, the model snapshot, and design-time context creation | Data project, EF Core provider, and `Microsoft.EntityFrameworkCore.Design` |
| `WebApplication1` | Runs the application | Data project and migrations project |

The application needs a reference to the migrations project when it discovers or applies migrations at run time, for example by calling `Migrate`. If migrations are applied only by a deployment artifact and the application never loads them, that reference isn't required.

## Configure the projects

1. Create a class library for the migrations and add a reference to the project containing the `DbContext`.

2. Add the database provider and `Microsoft.EntityFrameworkCore.Design` to the migrations project. Mark the design package as a private development dependency:

   ```xml
   <ItemGroup>
     <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="...">
       <PrivateAssets>all</PrivateAssets>
       <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
     </PackageReference>
     <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="..." />
   </ItemGroup>

   <ItemGroup>
     <ProjectReference Include="..\WebApplication1.Data\WebApplication1.Data.csproj" />
   </ItemGroup>
   ```

3. Implement [`IDesignTimeDbContextFactory<TContext>`](xref:core/cli/dbcontext-creation#from-a-design-time-factory) in the migrations project. The factory allows the tools to create the context without running the application project:

   [!code-csharp[](../../../../samples/core/Schemas/ThreeProjectMigrations/WebApplication1.Migrations/ApplicationDbContextFactory.cs#snippet_DesignTimeFactory)]

   Keep design-time provider and model configuration consistent with the runtime configuration. The sample accepts an optional connection string argument and uses a local development connection when no argument is supplied.

4. Configure the migrations assembly when registering the context at run time:

   [!code-csharp[](../../../../samples/core/Schemas/ThreeProjectMigrations/WebApplication1/Startup.cs#snippet_MigrationsAssembly)]

5. If the application applies migrations or otherwise discovers them at run time, add a normal reference from the application to the migrations project:

   ```xml
   <ItemGroup>
     <ProjectReference Include="..\WebApplication1.Migrations\WebApplication1.Migrations.csproj" />
   </ItemGroup>
   ```

   The data project must not reference the migrations project. That would create a circular dependency because the migrations project already references the data project.

6. If migrations already exist, move all migration files and the model snapshot to the migrations project and update their namespaces. When there are no existing migrations, the design-time factory allows the initial migration to be created directly in the migrations project.

## Use the tools

Use the migrations project as both the [target project and startup project](xref:core/cli/dotnet#target-project-and-startup-project). The target project receives generated files, while the startup project is built and executed by the tools. In this layout, using the migrations project for both prevents the tools from executing application startup code.

### [.NET CLI](#tab/dotnet-core-cli)

Run these commands from the solution directory:

```dotnetcli
dotnet ef migrations add NewMigration \
    --project WebApplication1.Migrations \
    --startup-project WebApplication1.Migrations
```

The same project options apply to other commands:

```dotnetcli
dotnet ef migrations list \
    --project WebApplication1.Migrations \
    --startup-project WebApplication1.Migrations

dotnet ef migrations script --output artifacts/migrations.sql \
    --project WebApplication1.Migrations \
    --startup-project WebApplication1.Migrations

dotnet ef migrations bundle --output artifacts/efbundle \
    --project WebApplication1.Migrations \
    --startup-project WebApplication1.Migrations
```

Starting with EF Core 11, repeated project options can be stored in [`.config/dotnet-ef.json`](xref:core/cli/dotnet#configuration-file).

### [Visual Studio](#tab/vs)

Use the migrations project for both `-Project` and `-StartupProject`:

```powershell
Add-Migration NewMigration `
    -Project WebApplication1.Migrations `
    -StartupProject WebApplication1.Migrations
```

The same parameters can be passed to `Get-Migration`, `Script-Migration`, `Bundle-Migration`, and the other Package Manager Console commands.

***

Build the migrations project before running commands with `--no-build`, or before another process consumes its output. A normal `dotnet ef` command builds the target and startup projects automatically.

## Platform-specific applications

Don't use a platform-specific application project as the startup project for EF tools. Mobile, browser, desktop, function, and RID-specific projects can require a workload or native host that `dotnet ef` can't execute. Starting with EF Core 11, the tools warn when a platform-specific startup project is used.

Use the layout described above for .NET MAUI, WinUI, Blazor WebAssembly, Azure Functions, and similar applications:

1. Put the context and entity types in a shared data project.
2. Put migrations and `IDesignTimeDbContextFactory<TContext>` in a normal cross-platform .NET project.
3. Run the tools with the migrations project as the target and startup project.
4. Reference the migrations project from the application only if the application loads or applies migrations at run time.

Direct tooling support for Xamarin and MAUI platform projects isn't planned; see [dotnet/efcore#7152](https://github.com/dotnet/efcore/issues/7152). Xamarin applications should first be [upgraded to .NET MAUI](/dotnet/maui/migration).

### Process architecture

The process running the tools must be able to load every design-time assembly. A 64-bit Visual Studio or .NET process can't load an x86-only startup assembly, and the same constraint applies to Arm64 and other architectures. Prefer an AnyCPU migrations project. If design-time dependencies require a specific architecture, invoke a matching .NET SDK explicitly.

The design-time process architecture is separate from the deployment target. When creating a bundle, use `--target-runtime` or `-TargetRuntime` to generate an artifact for the deployment RID, such as `linux-arm64` or `osx-arm64`.
