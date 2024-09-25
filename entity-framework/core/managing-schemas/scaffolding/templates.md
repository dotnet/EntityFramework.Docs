---
title: Custom Reverse Engineering Templates - EF Core
description: Using T4 text templates to customize the scaffolded code when reverse engineering an Entity Framework Core model from a database
author: SamMonoRT
ms.date: 08/16/2022
uid: core/managing-schemas/scaffolding/templates
---
# Custom Reverse Engineering Templates

> [!NOTE]
> This feature was added in EF Core 7.

While [reverse engineering](xref:core/managing-schemas/scaffolding), Entity Framework Core strives to scaffold good, general-purpose code that can be used in a variety of app types and uses [common coding conventions](/dotnet/csharp/fundamentals/coding-style/coding-conventions) for a consistent look and a familiar feel. Sometimes, however, more specialized code and alternative coding styles are desirable. This article shows how to customize the scaffolded code using [T4 text templates](/visualstudio/modeling/code-generation-and-t4-text-templates).

## Prerequisites

This article assumes you're familiar with [reverse engineering in EF Core](xref:core/managing-schemas/scaffolding). If not, please review that article before proceeding.

## Adding the default templates

The first step to customizing the scaffolded code is to add the default templates to your project. The default templates are the ones used internally by EF Core when reverse engineering. They provide a starting point for you to begin customizing the scaffolded code.

Start by installing the EF Core template package for `dotnet new`:

```dotnetcli
dotnet new install Microsoft.EntityFrameworkCore.Templates
```

Now you can add the default templates to your project. Do this by running the following command from your project directory.

```dotnetcli
dotnet new ef-templates
```

This command adds the following files to your project.

- CodeTemplates/
  - EFCore/
    - DbContext.t4
    - EntityType.t4

The `DbContext.t4` template is used to scaffold a DbContext class for the database, and the `EntityType.t4` template is used to scaffold entity type classes for each table and view in the database.

> [!TIP]
> The .t4 extension is used (instead of .tt) to prevent Visual Studio from transforming the templates. The templates will be transformed by EF Core instead.

## Introduction to T4

Let's open the `DbContext.t4` template and inspect its contents. This file is a [T4 text template](/visualstudio/modeling/writing-a-t4-text-template). T4 is a language for generating text using .NET. The following code is for illustrative purposes only; it does not represent the full contents of the file.

> [!IMPORTANT]
> T4 text templates--especially ones that generate code--can be difficult to read without syntax highlighting. If necessary, search for an extension to your code editor that enables T4 syntax highlighting.

```T4
<#@ template hostSpecific="true" #>
<#@ assembly name="Microsoft.EntityFrameworkCore.Design" #>
<#@ parameter name="NamespaceHint" type="System.String" #>
<#@ import namespace="Microsoft.EntityFrameworkCore" #>
<#
    if (!string.IsNullOrEmpty(NamespaceHint))
    {
#>
namespace <#= NamespaceHint #>;
```

The first few lines that begin with `<#@` are called directives. They affect how the template is transformed. The following table briefly describes each kind of directive used.

Directive   | Description
----------- | -----------
`template`  | Specifies hostSpecific="true" which enables using the `Host` property inside the template to access EF Core services.
`assembly`  | Adds assembly references required to compile the template.
`parameter` | Declares parameters that will be passed in by EF Core when transforming the template.
`import`    | Like C# using directives, brings namespaces into scope for the template code.

After the directives, the next section of `DbContext.t4` is called a control block. A standard control block begins with `<#` and ends with `#>`. The code inside of it will be executed when transforming the template. For a list of properties and methods available inside control blocks, see the [TextTransformation](/dotnet/api/microsoft.visualstudio.texttemplating.texttransformation) class.

Anything outside of a control block will be copied directly to the template output.

An expression control block begins with `<#=`. The code inside of it will be evaluated and the result will be added to the template output. These are similar to C# interpolated string arguments.

For a more detailed and complete explanation of the T4 syntax, see [Writing a T4 Text Template](/visualstudio/modeling/writing-a-t4-text-template).

## Customize the entity types

Let's walk through what it's like to customize a template. By default, EF Core generates the following code for collection navigation properties.

```C#
public virtual ICollection<Album> Albums { get; } = new List<Album>();
```

Using `List<T>` is a good default for most applications. However, if you're using a XAML-based framework like WPF, WinUI, or .NET MAUI, you often want to use `ObservableCollection<T>` instead to enable data binding.

Open the `EntityType.t4` template and find where it generates `List<T>`. It looks like this:

```T4
    if (navigation.IsCollection)
    {
#>
    public virtual ICollection<<#= targetType #>> <#= navigation.Name #> { get; } = new List<<#= targetType #>>();
<#
    }
```

Replace List with ObservableCollection.

```T4
public virtual ICollection<<#= targetType #>> <#= navigation.Name #> { get; } = new ObservableCollection<<#= targetType #>>();
```

We also need to add a `using` directive to the scaffolded code. The usings are specified in a list near the top of the template. Add `System.Collections.ObjectModel` to the list.

```C#
var usings = new List<string>
{
    "System",
    "System.Collections.Generic",
    "System.Collections.ObjectModel"
};
```

Test the changes by using the reverse engineering commands. The templates inside your project are used automatically by the commands.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer
```

If you've ran the command previously, add the `--force` option to overwrite the existing files.

### [Visual Studio](#tab/vs)

```powershell
Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook' Microsoft.EntityFrameworkCore.SqlServer
```

If you've ran the command previously, add the `-Force` switch to overwrite the existing files.

***

If you did everything correctly, the collection navigation properties should now use `ObservableCollection<T>`.

```C#
public virtual ICollection<Album> Albums { get; } = new ObservableCollection<Album>();
```

## Updating templates

When you add the default templates to your project, it creates a copy of them based on that version of EF Core. As bugs are fixed and features are added in subsequent versions of EF Core, your templates may become out of date. You should review the changes made in the EF Core templates and merge them into your customized templates.

One way to review the changes made to the EF Core templates is to use git to compare them between versions. The following command will clone the EF Core repository and generate a diff of these files between versions 7.0.0 and 8.0.0.

```console
git clone --no-checkout https://github.com/dotnet/efcore.git
cd efcore
git diff v7.0.0 v8.0.0 -- src/EFCore.Design/Scaffolding/Internal/CSharpDbContextGenerator.tt src/EFCore.Design/Scaffolding/Internal/CSharpEntityTypeGenerator.tt
```

Another way to review the changes is to download the two versions of [Microsoft.EntityFrameworkCore.Templates](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Templates) from NuGet, extract their contents (you can change the file extensions to .zip), and compare those files.

Before adding the default templates to a new project, remember to update to the latest EF Core template package.

```dotnetcli
dotnet new update
```

## Advanced usage

### Ignoring the input model

The `Model` and `EntityType` parameters represent one possible way of mapping to the database. You can choose to ignore or change parts of the model. For example, the navigation names we provide may not be ideal, and you can replace them with your own when scaffolding the code. Other things like constraint names and index filters are only used by Migrations and can safely be omitted from the model if you don't intend to use Migrations with the scaffolded code. Likewise, you may want to omit sequences or default constraints if they're not used by your app.

When making advanced changes like this, just make sure the resulting model remains compatible with the database. Reviewing the SQL generated by `dbContext.Database.GenerateCreateScript()` is a good way to validate this.

### Entity configuration classes

For large models, the OnModelCreating method of the DbContext class can become unmanageably large. One way to address this is to use `IEntityTypeConfiguration<T>` classes. See [Creating and configuring a model](xref:core/modeling/index#grouping-configuration) for more information about these classes.

To scaffold these classes, you can use a third template called `EntityTypeConfiguration.t4`. Like the `EntityType.t4` template, it gets used for each entity type in the model and uses the `EntityType` template parameter.

### Scaffolding other types of files

The primary purpose of reverse engineering in EF Core is to scaffold a DbContext and entity types. However, there's nothing in the tools that require you to actually scaffold code. For example, you could instead scaffold an entity relationship diagram using [Mermaid](https://mermaid-js.github.io/).

````T4
<#@ output extension=".md" #>
<#@ assembly name="Microsoft.EntityFrameworkCore" #>
<#@ assembly name="Microsoft.EntityFrameworkCore.Relational" #>
<#@ assembly name="Microsoft.EntityFrameworkCore.Design" #>
<#@ parameter name="Model" type="Microsoft.EntityFrameworkCore.Metadata.IModel" #>
<#@ parameter name="Options" type="Microsoft.EntityFrameworkCore.Scaffolding.ModelCodeGenerationOptions" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="Microsoft.EntityFrameworkCore" #>
# <#= Options.ContextName #>

```mermaid
erDiagram
<#
    foreach (var entityType in Model.GetEntityTypes().Where(e => !e.IsSimpleManyToManyJoinEntityType()))
    {
#>
    <#= entityType.Name #> {
    }
<#
        foreach (var foreignKey in entityType.GetForeignKeys())
        {
#>
    <#= entityType.Name #> <#= foreignKey.IsUnique ? "|" : "}" #>o--<#= foreignKey.IsRequired ? "|" : "o" #>| <#= foreignKey.PrincipalEntityType.Name #> : "<#= foreignKey.GetConstraintName() #>"
<#
        }

        foreach (var skipNavigation in entityType.GetSkipNavigations().Where(n => n.IsLeftNavigation()))
        {
#>
    <#= entityType.Name #> }o--o{ <#= skipNavigation.TargetEntityType.Name #> : <#= skipNavigation.JoinEntityType.Name #>
<#
        }
    }
#>
```
````
