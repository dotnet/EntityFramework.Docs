---
title: Custom Migrations Operations - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/07/2017
uid: core/managing-schemas/migrations/operations
---
Custom Migrations Operations
============================
The MigrationBuilder API allows you to perform many different kinds of operations during a migration, but it's far from
exhaustive. However, the API is also extensible allowing you to define your own operations. There are two ways to extend
the API: Using the `Sql()` method, or by defining custom `MigrationOperation` objects.

To illustrate, let's look at implementing an operation that creates a database user using each approach. In our
migrations, we want to enable writing the following code:

``` csharp
migrationBuilder.CreateUser("SQLUser1", "Password");
```

Using MigrationBuilder.Sql()
----------------------------
The easiest way to implement a custom operation is to define an extension method that calls `MigrationBuilder.Sql()`.
Here is an example that generates the appropriate Transact-SQL.

``` csharp
static MigrationBuilder CreateUser(
    this MigrationBuilder migrationBuilder,
    string name,
    string password)
    => migrationBuilder.Sql($"CREATE USER {name} WITH PASSWORD '{password}';");
```

If your migrations need to support multiple database providers, you can use the `MigrationBuilder.ActiveProvider`
property. Here's an example supporting both Microsoft SQL Server and PostgreSQL.

``` csharp
static MigrationBuilder CreateUser(
    this MigrationBuilder migrationBuilder,
    string name,
    string password)
{
    switch (migrationBuilder.ActiveProvider)
    {
        case "Npgsql.EntityFrameworkCore.PostgreSQL":
            return migrationBuilder
                .Sql($"CREATE USER {name} WITH PASSWORD '{password}';");

        case "Microsoft.EntityFrameworkCore.SqlServer":
            return migrationBuilder
                .Sql($"CREATE USER {name} WITH PASSWORD = '{password}';");
    }

    return migrationBuilder;
}
```

This approach only works if you know every provider where your custom operation will be applied.

Using a MigrationOperation
---------------------------
To decouple the custom operation from the SQL, you can define your own `MigrationOperation` to represent it. The
operation is then passed to the provider so it can determine the appropriate SQL to generate.

``` csharp
class CreateUserOperation : MigrationOperation
{
    public string Name { get; set; }
    public string Password { get; set; }
}
```

With this approach, the extension method just needs to add one of these operations to `MigrationBuilder.Operations`.

``` csharp
static MigrationBuilder CreateUser(
    this MigrationBuilder migrationBuilder,
    string name,
    string password)
{
    migrationBuilder.Operations.Add(
        new CreateUserOperation
        {
            Name = name,
            Password = password
        });

    return migrationBuilder;
}
```

This approach requires each provider to know how to generate SQL for this operation in their `IMigrationsSqlGenerator`
service. Here is an example overriding the SQL Server's generator to handle the new operation.

``` csharp
class MyMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
{
    public MyMigrationsSqlGenerator(
        MigrationsSqlGeneratorDependencies dependencies,
        IMigrationsAnnotationProvider migrationsAnnotations)
        : base(dependencies, migrationsAnnotations)
    {
    }

    protected override void Generate(
        MigrationOperation operation,
        IModel model,
        MigrationCommandListBuilder builder)
    {
        if (operation is CreateUserOperation createUserOperation)
        {
            Generate(createUserOperation, builder);
        }
        else
        {
            base.Generate(operation, model, builder);
        }
    }

    private void Generate(
        CreateUserOperation operation,
        MigrationCommandListBuilder builder)
    {
        var sqlHelper = Dependencies.SqlGenerationHelper;
        var stringMapping = Dependencies.TypeMappingSource.FindMapping(typeof(string));

        builder
            .Append("CREATE USER ")
            .Append(sqlHelper.DelimitIdentifier(operation.Name))
            .Append(" WITH PASSWORD = ")
            .Append(stringMapping.GenerateSqlLiteral(operation.Password))
            .AppendLine(sqlHelper.StatementTerminator)
            .EndCommand();
    }
}
```

Replace the default migrations sql generator service with the updated one.

``` csharp
protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options
        .UseSqlServer(connectionString)
        .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();
```
