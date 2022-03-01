---
title: Model Bulk Configuration - EF Core
description: How to apply bulk configuration during model building in Entity Framework Core
author: AndriySvyryd
ms.date: 11/05/2021
uid: core/modeling/bulk-configuration
---
# Model bulk configuration

When an aspect needs to be configured in the same way across multiple entity types, the following techniques allow to reduce code duplication and consolidate the logic.

See the [full sample project](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/BulkConfiguration) containing the code snippets presented below.

## Bulk configuration in OnModelCreating

Every builder object returned from <xref:Microsoft.EntityFrameworkCore.ModelBuilder> exposes a <xref:Microsoft.EntityFrameworkCore.ModelBuilder.Model> or `Metadata` property that provides a low-level access to the objects that comprise the model. In particular, there are methods that allow you to iterate over specific objects in the model and apply common configuration to them.

In the following example the model contains a custom value type `Currency`:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/Currency.cs?name=Currency)]

Properties of this type are not discovered by default as the current EF provider doesn't know how to map it to a database type. This snippet of `OnModelCreating` adds all properties of the type `Currency` and configures a value converter to a supported type - `decimal`:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/MetadataAPIContext.cs?name=MetadataAPI)]

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/CurrencyConverter.cs?name=CurrencyConverter)]

### Drawbacks of the Metadata API

- Unlike Fluent API, every modification to the model needs to be done explicitly. For example, if some of the `Currency` properties were configured as navigations by a convention then you need to first remove the navigation referencing the CLR property before adding an entity type property for it. [#9117](https://github.com/dotnet/efcore/issues/9117) will improve this.
- The conventions run after each change. If you remove a navigation discovered by a convention then the convention will run again and could add it back. To prevent this from happening you would need to either delay the conventions until after the property is added by calling <xref:Microsoft.EntityFrameworkCore.Metadata.Conventions.IConventionContext.DelayConventions> and later disposing the returned object or to mark the CLR property as ignored using <xref:Microsoft.EntityFrameworkCore.Metadata.IMutableModel.AddIgnored%2A>.
- Entity types might be added after this iteration happens and the configuration won't be applied to them. This can usually be prevented by placing this code at the end of `OnModelCreating`, but if you have two interdependent sets of configurations there might not be an order that will allow them to be applied consistently.

## Pre-convention configuration

EF Core 6.0 allows the mapping configuration to be specified once for a given CLR type; that configuration is then applied to all properties of that type in the model as they are discovered. This is called "pre-convention model configuration", since it configures aspects of the model that are then used by the model building conventions. Such configuration is applied by overriding <xref:Microsoft.EntityFrameworkCore.DbContext.ConfigureConventions%2A> on the type derived from <xref:Microsoft.EntityFrameworkCore.DbContext>.

This example shows how configure all properties of type `Currency` to have a value converter:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/CurrencyContext.cs?name=ConfigureConventions)]

And this example shows how to configure some facets on all properties of type `string`:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/PreConventionContext.cs?name=StringFacets)]

> [!NOTE]
> The type specified in a call from `ConfigureConventions` can be a base type, an interface or a generic type definition. All matching configurations will be applied in order from the least specific:
>
> 1. Interface
> 2. Base type
> 3. Generic type definition
> 4. Non-nullable value type
> 5. Exact type

### Ignoring types

Pre-convention configuration also allows to ignore a type and prevent it from being discovered by conventions either as an entity type or as a property on an entity type:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/PreConventionContext.cs?name=IgnoreInterface)]

### Default type mapping

Generally, EF is able to translate queries with constants of a type that is not supported by the provider, as long as you have specified a value converter for a property of this type. However, in queries that don't involve any properties of this type, there is no way for EF to find the correct value converter. In this case, it's possible to call <xref:Microsoft.EntityFrameworkCore.ModelConfigurationBuilder.DefaultTypeMapping%2A> to add or override a provider type mapping:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/PreConventionContext.cs?name=DefaultTypeMapping)]

### Limitations of pre-convention configuration

- Many aspects cannot be configured with this approach. [#6787](https://github.com/dotnet/efcore/issues/6787) will expand this to more types.
- Currently the configuration is only determined by the CLR type. [#20418](https://github.com/dotnet/efcore/issues/20418) would allow custom predicates.
- This configuration is performed before a model is created. If there are any conflicts that arise when applying it, the exception stack trace will not contain the `ConfigureConventions` method, so it might be harder to find the cause.
