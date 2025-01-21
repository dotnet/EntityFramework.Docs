---
title: Value Conversions - EF Core
description: Configuring value converters in an Entity Framework Core model
author: SamMonoRT
ms.date: 01/16/2021
uid: core/modeling/value-conversions
---
# Value Conversions

Value converters allow property values to be converted when reading from or writing to the database. This conversion can be from one value to another of the same type (for example, encrypting strings) or from a value of one type to a value of another type (for example, converting enum values to and from strings in the database.)

> [!TIP]
> You can run and debug into all the code in this document by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/ValueConversions/).

## Overview

Value converters are specified in terms of a `ModelClrType` and a `ProviderClrType`. The model type is the .NET type of the property in the entity type. The provider type is the .NET type understood by the database provider. For example, to save enums as strings in the database, the model type is the type of the enum, and the provider type is `String`. These two types can be the same.

Conversions are defined using two `Func` expression trees: one from `ModelClrType` to `ProviderClrType` and the other from `ProviderClrType` to `ModelClrType`. Expression trees are used so that they can be compiled into the database access delegate for efficient conversions. The expression tree may contain a simple call to a conversion method for complex conversions.

> [!NOTE]
> A property that has been configured for value conversion may also need to specify a <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer`1>. See the examples below, and the [Value Comparers](xref:core/modeling/value-comparers) documentation for more information.

## Configuring a value converter

Value conversions are configured in <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating*?displayProperty=nameWithType>. For example, consider an enum and entity type defined as:

<!--
        public class Rider
        {
            public int Id { get; set; }
            public EquineBeast Mount { get; set; }
        }

        public enum EquineBeast
        {
            Donkey,
            Mule,
            Horse,
            Unicorn
        }
-->
[!code-csharp[BeastAndRider](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=BeastAndRider)]

Conversions can be configured in <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating*> to store the enum values as strings such as "Donkey", "Mule", etc. in the database; you simply need to provide one function which converts from the `ModelClrType` to the `ProviderClrType`, and another for the opposite conversion:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<Rider>()
                    .Property(e => e.Mount)
                    .HasConversion(
                        v => v.ToString(),
                        v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));
            }
-->
[!code-csharp[ExplicitConversion](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ExplicitConversion)]

> [!NOTE]
> A `null` value will never be passed to a value converter. A null in a database column is always a null in the entity instance, and vice-versa. This makes the implementation of conversions easier and allows them to be shared amongst nullable and non-nullable properties. See [GitHub issue #13850](https://github.com/dotnet/efcore/issues/13850) for more information.

### Bulk-configuring a value converter

It's common for the same value converter to be configured for every property that uses the relevant CLR type. Rather than doing this manually for each property, you can use [pre-convention model configuration](xref:core/modeling/bulk-configuration#pre-convention-configuration) to do this once for your entire model. To do this, define your value converter as a class:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/CurrencyConverter.cs?name=CurrencyConverter)]

Then, override <xref:Microsoft.EntityFrameworkCore.DbContext.ConfigureConventions*> in your context type and configure the converter as follows:

[!code-csharp[Main](../../../samples/core/Modeling/BulkConfiguration/CurrencyContext.cs?name=ConfigureConventions)]

## Pre-defined conversions

EF Core contains many pre-defined conversions that avoid the need to write conversion functions manually. Instead, EF Core will pick the conversion to use based on the property type in the model and the requested database provider type.

For example, enum to string conversions are used as an example above, but EF Core will actually do this automatically when the provider type is configured as `string` using the generic type of <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder.HasConversion*>:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<Rider>()
                    .Property(e => e.Mount)
                    .HasConversion<string>();
            }
-->
[!code-csharp[ConversionByClrType](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByClrType)]

The same thing can be achieved by explicitly specifying the database column type. For example, if the entity type is defined like so:

### [Data Annotations](#tab/data-annotations)

<!--
        public class Rider2
        {
            public int Id { get; set; }

            [Column(TypeName = "nvarchar(24)")]
            public EquineBeast Mount { get; set; }
        }
-->
[!code-csharp[ConversionByDatabaseType](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByDatabaseType)]

### [Fluent API](#tab/fluent-api)

<!--
                modelBuilder
                    .Entity<Rider2>()
                    .Property(e => e.Mount)
                    .HasColumnType("nvarchar(24)");
-->
[!code-csharp[ConversionByDatabaseTypeFluent](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByDatabaseTypeFluent)]

***

Then the enum values will be saved as strings in the database without any further configuration in <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating*>.

## The ValueConverter class

Calling <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder.HasConversion*> as shown above will create a <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter`2> instance and set it on the property. The `ValueConverter` can instead be created explicitly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var converter = new ValueConverter<EquineBeast, string>(
                    v => v.ToString(),
                    v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

                modelBuilder
                    .Entity<Rider>()
                    .Property(e => e.Mount)
                    .HasConversion(converter);
            }
-->
[!code-csharp[ConversionByConverterInstance](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByConverterInstance)]

This can be useful when multiple properties use the same conversion.

## Built-in converters

As mentioned above, EF Core ships with a set of pre-defined <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter`2> classes, found in the <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion> namespace. In many cases EF will choose the appropriate built-in converter based on the type of the property in the model and the type requested in the database, as shown above for enums. For example, using `.HasConversion<int>()` on a `bool` property will cause EF Core to convert bool values to numerical zero and one values:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<User>()
                    .Property(e => e.IsActive)
                    .HasConversion<int>();
            }
-->
[!code-csharp[ConversionByBuiltInBoolToInt](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByBuiltInBoolToInt)]

This is functionally the same as creating an instance of the built-in <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.BoolToZeroOneConverter`1> and setting it explicitly:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var converter = new BoolToZeroOneConverter<int>();

                modelBuilder
                    .Entity<User>()
                    .Property(e => e.IsActive)
                    .HasConversion(converter);
            }
-->
[!code-csharp[ConversionByBuiltInBoolToIntExplicit](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByBuiltInBoolToIntExplicit)]

The following table summarizes commonly-used pre-defined conversions from model/property types to database provider types. In the table `any_numeric_type` means one of `int`, `short`, `long`, `byte`, `uint`, `ushort`, `ulong`, `sbyte`, `char`, `decimal`, `float`, or `double`.

| Model/property type | Provider/database type | Conversion                                                | Usage
|:--------------------|------------------------|-----------------------------------------------------------|------
| bool                | any_numeric_type       | False/true to 0/1                                         | `.HasConversion<any_numeric_type>()`
|                     | any_numeric_type       | False/true to any two numbers                             | Use <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.BoolToTwoValuesConverter`1>
|                     | string                 | False/true to "N"/"Y"                                     | `.HasConversion<string>()`
|                     | string                 | False/true to any two strings                             | Use <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.BoolToStringConverter>
| any_numeric_type    | bool                   | 0/1 to false/true                                         | `.HasConversion<bool>()`
|                     | any_numeric_type       | Simple cast                                               | `.HasConversion<any_numeric_type>()`
|                     | string                 | The number as a string                                    | `.HasConversion<string>()`
| Enum                | any_numeric_type       | The numeric value of the enum                             | `.HasConversion<any_numeric_type>()`
|                     | string                 | The string representation of the enum value               | `.HasConversion<string>()`
| string              | bool                   | Parses the string as a bool                               | `.HasConversion<bool>()`
|                     | any_numeric_type       | Parses the string as the given numeric type               | `.HasConversion<any_numeric_type>()`
|                     | char                   | The first character of the string                         | `.HasConversion<char>()`
|                     | DateTime               | Parses the string as a DateTime                           | `.HasConversion<DateTime>()`
|                     | DateTimeOffset         | Parses the string as a DateTimeOffset                     | `.HasConversion<DateTimeOffset>()`
|                     | TimeSpan               | Parses the string as a TimeSpan                           | `.HasConversion<TimeSpan>()`
|                     | Guid                   | Parses the string as a Guid                               | `.HasConversion<Guid>()`
|                     | byte[]                 | The string as UTF8 bytes                                  | `.HasConversion<byte[]>()`
| char                | string                 | A single character string                                 | `.HasConversion<string>()`
| DateTime            | long                   | Encoded date/time preserving DateTime.Kind                | `.HasConversion<long>()`
|                     | long                   | Ticks                                                     | Use <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeToTicksConverter>
|                     | string                 | Invariant culture date/time string                        | `.HasConversion<string>()`
| DateTimeOffset      | long                   | Encoded date/time with offset                             | `.HasConversion<long>()`
|                     | string                 | Invariant culture date/time string with offset            | `.HasConversion<string>()`
| TimeSpan            | long                   | Ticks                                                     | `.HasConversion<long>()`
|                     | string                 | Invariant culture time span string                        | `.HasConversion<string>()`
| Uri                 | string                 | The URI as a string                                       | `.HasConversion<string>()`
| PhysicalAddress     | string                 | The address as a string                                   | `.HasConversion<string>()`
|                     | byte[]                 | Bytes in big-endian network order                         | `.HasConversion<byte[]>()`
| IPAddress           | string                 | The address as a string                                   | `.HasConversion<string>()`
|                     | byte[]                 | Bytes in big-endian network order                         | `.HasConversion<byte[]>()`
| Guid                | string                 | The GUID in 'dddddddd-dddd-dddd-dddd-dddddddddddd' format | `.HasConversion<string>()`
|                     | byte[]                 | Bytes in .NET binary serialization order                  | `.HasConversion<byte[]>()`

Note that these conversions assume that the format of the value is appropriate for the conversion. For example, converting strings to numbers will fail if the string values cannot be parsed as numbers.

The full list of built-in converters is:

* Converting bool properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.BoolToStringConverter> - Bool to strings such as "N" and "Y"
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.BoolToTwoValuesConverter`1> - Bool to any two values
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.BoolToZeroOneConverter`1> - Bool to zero and one
* Converting byte array properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.BytesToStringConverter> - Byte array to Base64-encoded string
* Any conversion that requires only a type-cast
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.CastingConverter`2> - Conversions that require only a type cast
* Converting char properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.CharToStringConverter> - Char to single character string
* Converting <xref:System.DateTimeOffset> properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeOffsetToBinaryConverter> - <xref:System.DateTimeOffset> to binary-encoded 64-bit value
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeOffsetToBytesConverter> - <xref:System.DateTimeOffset> to byte array
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeOffsetToStringConverter> - <xref:System.DateTimeOffset> to string
* Converting <xref:System.DateTime> properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeToBinaryConverter> - <xref:System.DateTime> to 64-bit value including DateTimeKind
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeToStringConverter> - <xref:System.DateTime> to string
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeToTicksConverter> - <xref:System.DateTime> to ticks
* Converting enum properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToNumberConverter`2> - Enum to underlying number
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToStringConverter`1> - Enum to string
* Converting <xref:System.Guid> properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.GuidToBytesConverter> - <xref:System.Guid> to byte array
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.GuidToStringConverter> - <xref:System.Guid> to string
* Converting <xref:System.Net.IPAddress> properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.IPAddressToBytesConverter> - <xref:System.Net.IPAddress> to byte array
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.IPAddressToStringConverter> - <xref:System.Net.IPAddress> to string
* Converting numeric (int, double, decimal, etc.) properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.NumberToBytesConverter`1> - Any numerical value to byte array
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.NumberToStringConverter`1> - Any numerical value to string
* Converting <xref:System.Net.NetworkInformation.PhysicalAddress> properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.PhysicalAddressToBytesConverter> - <xref:System.Net.NetworkInformation.PhysicalAddress> to byte array
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.PhysicalAddressToStringConverter> - <xref:System.Net.NetworkInformation.PhysicalAddress> to string
* Converting string properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToBoolConverter> - Strings such as "N" and "Y" to bool
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToBytesConverter> - String to UTF8 bytes
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToCharConverter> - String to character
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToDateTimeConverter> - String to <xref:System.DateTime>
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToDateTimeOffsetConverter> - String to <xref:System.DateTimeOffset>
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToEnumConverter`1> - String to enum
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToGuidConverter> - String to <xref:System.Guid>
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToNumberConverter`1> - String to numeric type
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToTimeSpanConverter> - String to <xref:System.TimeSpan>
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.StringToUriConverter> - String to <xref:System.Uri>
* Converting <xref:System.TimeSpan> properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.TimeSpanToStringConverter> - <xref:System.TimeSpan> to string
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.TimeSpanToTicksConverter> - <xref:System.TimeSpan> to ticks
* Converting <xref:System.Uri> properties:
  * <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.UriToStringConverter> - <xref:System.Uri> to string

Note that all the built-in converters are stateless and so a single instance can be safely shared by multiple properties.

## Column facets and mapping hints

Some database types have facets that modify how the data is stored. These include:

* Precision and scale for decimals and date/time columns
* Size/length for binary and string columns
* Unicode for string columns

These facets can be configured in the normal way for a property that uses a value converter, and will apply to the converted database type. For example, when converting from an enum to strings, we can specify that the database column should be non-Unicode and store up to 20 characters:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<Rider>()
                    .Property(e => e.Mount)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            }
-->
[!code-csharp[ConversionByClrTypeWithFacets](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByClrTypeWithFacets)]

Or, when creating the converter explicitly:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var converter = new ValueConverter<EquineBeast, string>(
                    v => v.ToString(),
                    v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

                modelBuilder
                    .Entity<Rider>()
                    .Property(e => e.Mount)
                    .HasConversion(converter)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            }
-->
[!code-csharp[ConversionByConverterInstanceWithFacets](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByConverterInstanceWithFacets)]

This results in a `varchar(20)` column when using EF Core migrations against SQL Server:

```sql
CREATE TABLE [Rider] (
    [Id] int NOT NULL IDENTITY,
    [Mount] varchar(20) NOT NULL,
    CONSTRAINT [PK_Rider] PRIMARY KEY ([Id]));
```

However, if by default all `EquineBeast` columns should be `varchar(20)`, then this information can be given to the value converter as a <xref:Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints>. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var converter = new ValueConverter<EquineBeast, string>(
                    v => v.ToString(),
                    v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v),
                    new ConverterMappingHints(size: 20, unicode: false));

                modelBuilder
                    .Entity<Rider>()
                    .Property(e => e.Mount)
                    .HasConversion(converter);
            }
-->
[!code-csharp[ConversionByConverterInstanceWithMappingHints](../../../samples/core/Modeling/ValueConversions/EnumToStringConversions.cs?name=ConversionByConverterInstanceWithMappingHints)]

Now any time this converter is used, the database column will be non-unicode with a max length of 20. However, these are only hints since they are overridden by any facets explicitly set on the mapped property.

## Examples

### Simple value objects

This example uses a simple type to wrap a primitive type. This can be useful when you want the type in your model to be more specific (and hence more type-safe) than a primitive type. In this example, that type is `Dollars`, which wraps the decimal primitive:

<!--
        public readonly struct Dollars
        {
            public Dollars(decimal amount) 
                => Amount = amount;
            
            public decimal Amount { get; }

            public override string ToString() 
                => $"${Amount}";
        }
-->
[!code-csharp[SimpleValueObject](../../../samples/core/Modeling/ValueConversions/SimpleValueObject.cs?name=SimpleValueObject)]

This can be used in an entity type:

<!--
        public class Order
        {
            public int Id { get; set; }

            public Dollars Price { get; set; }
        }
-->
[!code-csharp[SimpleValueObjectModel](../../../samples/core/Modeling/ValueConversions/SimpleValueObject.cs?name=SimpleValueObjectModel)]

And converted to the underlying `decimal` when stored in the database:

<!--
                modelBuilder.Entity<Order>()
                    .Property(e => e.Price)
                    .HasConversion(
                        v => v.Amount,
                        v => new Dollars(v));
-->
[!code-csharp[ConfigureImmutableStructProperty](../../../samples/core/Modeling/ValueConversions/SimpleValueObject.cs?name=ConfigureImmutableStructProperty)]

> [!NOTE]
> This value object is implemented as a [readonly struct](/dotnet/csharp/language-reference/builtin-types/struct). This means that EF Core can snapshot and compare values without issue. See [Value Comparers](xref:core/modeling/value-comparers) for more information.

### Composite value objects

In the previous example, the value object type contained only a single property. It is more common for a value object type to compose multiple properties that together form a domain concept. For example, a general `Money` type that contains both the amount and the currency:

<!--
        public readonly struct Money
        {
            [JsonConstructor]
            public Money(decimal amount, Currency currency)
            {
                Amount = amount;
                Currency = currency;
            }

            public override string ToString()
                => (Currency == Currency.UsDollars ? "$" : "£") + Amount;

            public decimal Amount { get; }
            public Currency Currency { get; }
        }

        public enum Currency
        {
            UsDollars,
            PoundsStirling
        }
-->
[!code-csharp[CompositeValueObject](../../../samples/core/Modeling/ValueConversions/CompositeValueObject.cs?name=CompositeValueObject)]

This value object can be used in an entity type as before:

<!--
        public class Order
        {
            public int Id { get; set; }

            public Money Price { get; set; }
        }
-->
[!code-csharp[CompositeValueObjectModel](../../../samples/core/Modeling/ValueConversions/CompositeValueObject.cs?name=CompositeValueObjectModel)]

Value converters can currently only convert values to and from a single database column. This limitation means that all property values from the object must be encoded into a single column value. This is typically handled by serializing the object as it goes into the database, and then deserializing it again on the way out. For example, using <xref:System.Text.Json>:

<!--
                modelBuilder.Entity<Order>()
                    .Property(e => e.Price)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, null),
                        v => JsonSerializer.Deserialize<Money>(v, null));
-->
[!code-csharp[ConfigureCompositeValueObject](../../../samples/core/Modeling/ValueConversions/CompositeValueObject.cs?name=ConfigureCompositeValueObject)]

> [!NOTE]
> We plan to allow mapping an object to multiple columns in a future version of EF Core, removing the need to use serialization here. This is tracked by [GitHub issue #13947](https://github.com/dotnet/efcore/issues/13947).

> [!NOTE]
> As with the previous example, this value object is implemented as a [readonly struct](/dotnet/csharp/language-reference/builtin-types/struct). This means that EF Core can snapshot and compare values without issue. See [Value Comparers](xref:core/modeling/value-comparers) for more information.

### Collections of primitives

Serialization can also be used to store a collection of primitive values. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Contents { get; set; }

            public ICollection<string> Tags { get; set; }
        }
-->
[!code-csharp[PrimitiveCollectionModel](../../../samples/core/Modeling/ValueConversions/PrimitiveCollection.cs?name=PrimitiveCollectionModel)]

Using <xref:System.Text.Json> again:

<!--
                modelBuilder.Entity<Post>()
                    .Property(e => e.Tags)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, null),
                        v => JsonSerializer.Deserialize<List<string>>(v, null),
                        new ValueComparer<ICollection<string>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => (ICollection<string>)c.ToList()));
-->
[!code-csharp[ConfigurePrimitiveCollection](../../../samples/core/Modeling/ValueConversions/PrimitiveCollection.cs?name=ConfigurePrimitiveCollection)]

`ICollection<string>` represents a mutable reference type. This means that a <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer`1> is needed so that EF Core can track and detect changes correctly. See [Value Comparers](xref:core/modeling/value-comparers) for more information.

### Collections of value objects

Combining the previous two examples together we can create a collection of value objects. For example, consider an `AnnualFinance` type that models blog finances for a single year:

<!--
        public readonly struct AnnualFinance
        {
            [JsonConstructor]
            public AnnualFinance(int year, Money income, Money expenses)
            {
                Year = year;
                Income = income;
                Expenses = expenses;
            }

            public int Year { get; }
            public Money Income { get; }
            public Money Expenses { get; }
            public Money Revenue => new Money(Income.Amount - Expenses.Amount, Income.Currency);
        }
-->
[!code-csharp[ValueObjectCollection](../../../samples/core/Modeling/ValueConversions/ValueObjectCollection.cs?name=ValueObjectCollection)]

This type composes several of the `Money` types we created previously:

<!--
        public readonly struct Money
        {
            [JsonConstructor]
            public Money(decimal amount, Currency currency)
            {
                Amount = amount;
                Currency = currency;
            }

            public override string ToString()
                => (Currency == Currency.UsDollars ? "$" : "£") + Amount;

            public decimal Amount { get; }
            public Currency Currency { get; }
        }

        public enum Currency
        {
            UsDollars,
            PoundsStirling
        }
-->
[!code-csharp[ValueObjectCollectionMoney](../../../samples/core/Modeling/ValueConversions/ValueObjectCollection.cs?name=ValueObjectCollectionMoney)]

We can then add a collection of `AnnualFinance` to our entity type:

<!--
        public class Blog
        {
            public int Id { get; set; }
            public string Name { get; set; }
            
            public IList<AnnualFinance> Finances { get; set; }
        }
-->
[!code-csharp[ValueObjectCollectionModel](../../../samples/core/Modeling/ValueConversions/ValueObjectCollection.cs?name=ValueObjectCollectionModel)]

And again use serialization to store this:

<!--
                modelBuilder.Entity<Blog>()
                    .Property(e => e.Finances)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, null),
                        v => JsonSerializer.Deserialize<List<AnnualFinance>>(v, null),
                        new ValueComparer<IList<AnnualFinance>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => (IList<AnnualFinance>)c.ToList()));
-->
[!code-csharp[ConfigureValueObjectCollection](../../../samples/core/Modeling/ValueConversions/ValueObjectCollection.cs?name=ConfigureValueObjectCollection)]

> [!NOTE]
> As before, this conversion requires a <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer`1>. See [Value Comparers](xref:core/modeling/value-comparers) for more information.

### Value objects as keys

Sometimes primitive key properties may be wrapped in value objects to add an additional level of type-safety in assigning values. For example, we could implement a key type for blogs, and a key type for posts:

<!--
        public readonly struct BlogKey
        {
            public BlogKey(int id) => Id = id;
            public int Id { get; }
        }

        public readonly struct PostKey
        {
            public PostKey(int id) => Id = id;
            public int Id { get; }
        }
-->
[!code-csharp[KeyValueObjects](../../../samples/core/Modeling/ValueConversions/KeyValueObjects.cs?name=KeyValueObjects)]

These can then be used in the domain model:

<!--
        public class Blog
        {
            public BlogKey Id { get; set; }
            public string Name { get; set; }

            public ICollection<Post> Posts { get; set; }
        }

        public class Post
        {
            public PostKey Id { get; set; }

            public string Title { get; set; }
            public string Content { get; set; }

            public BlogKey? BlogId { get; set; }
            public Blog Blog { get; set; }
        }
-->
[!code-csharp[KeyValueObjectsModel](../../../samples/core/Modeling/ValueConversions/KeyValueObjects.cs?name=KeyValueObjectsModel)]

Notice that `Blog.Id` cannot accidentally be assigned a `PostKey`, and `Post.Id` cannot accidentally be assigned a `BlogKey`. Similarly, the `Post.BlogId` foreign key property must be assigned a `BlogKey`.

> [!NOTE]
> Showing this pattern does not mean we recommend it. Carefully consider whether this level of abstraction is helping or hampering your development experience. Also, consider using navigations and generated keys instead of dealing with key values directly.

These key properties can then be mapped using value converters:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var blogKeyConverter = new ValueConverter<BlogKey, int>(
                    v => v.Id,
                    v => new BlogKey(v));

                modelBuilder.Entity<Blog>().Property(e => e.Id).HasConversion(blogKeyConverter);

                modelBuilder.Entity<Post>(
                    b =>
                        {
                            b.Property(e => e.Id).HasConversion(v => v.Id, v => new PostKey(v));
                            b.Property(e => e.BlogId).HasConversion(blogKeyConverter);
                        });
            }
-->
[!code-csharp[ConfigureKeyValueObjects](../../../samples/core/Modeling/ValueConversions/KeyValueObjects.cs?name=ConfigureKeyValueObjects)]

> [!NOTE]
> Key properties with conversions can only use generated key values starting with EF Core 7.0.

### Use ulong for timestamp/rowversion

SQL Server supports automatic [optimistic concurrency](xref:core/saving/concurrency) using [8-byte binary `rowversion`/`timestamp` columns](/sql/t-sql/data-types/rowversion-transact-sql). These are always read from and written to the database using an 8-byte array. However, byte arrays are a mutable reference type, which makes them somewhat painful to deal with. Value converters allow the `rowversion` to instead be mapped to a `ulong` property, which is much more appropriate and easy to use than the byte array. For example, consider a `Blog` entity with a ulong concurrency token:

<!--
        public class Blog
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ulong Version { get; set; }
        }
-->
[!code-csharp[ULongConcurrencyModel](../../../samples/core/Modeling/ValueConversions/ULongConcurrency.cs?name=ULongConcurrencyModel)]

This can be mapped to a SQL server `rowversion` column using a value converter:

<!--
                modelBuilder.Entity<Blog>()
                    .Property(e => e.Version)
                    .IsRowVersion()
                    .HasConversion<byte[]>();
-->
[!code-csharp[ConfigureULongConcurrency](../../../samples/core/Modeling/ValueConversions/ULongConcurrency.cs?name=ConfigureULongConcurrency)]

### Specify the DateTime.Kind when reading dates

SQL Server discards the <xref:System.DateTime.Kind*?displayProperty=nameWithType> flag when storing a <xref:System.DateTime> as a [`datetime`](/sql/t-sql/data-types/datetime-transact-sql) or [`datetime2`](/sql/t-sql/data-types/datetime2-transact-sql). This means that DateTime values coming back from the database always have a <xref:System.DateTimeKind> of `Unspecified`.

Value converters can be used in two ways to deal with this. First, EF Core has a value converter that creates an 8-byte opaque value which preserves the `Kind` flag. For example:

<!--
                modelBuilder.Entity<Post>()
                    .Property(e => e.PostedOn)
                    .HasConversion<long>();
-->
[!code-csharp[ConfigurePreserveDateTimeKind1](../../../samples/core/Modeling/ValueConversions/PreserveDateTimeKind.cs?name=ConfigurePreserveDateTimeKind1)]

This allows DateTime values with different `Kind` flags to be mixed in the database.

The problem with this approach is that the database no longer has recognizable `datetime` or `datetime2` columns. So instead it is common to always store UTC time (or, less commonly, always local time) and then either ignore the `Kind` flag or set it to the appropriate value using a value converter. For example, the converter below ensures that the `DateTime` value read from the database will have the <xref:System.DateTimeKind> `UTC`:

<!--
                modelBuilder.Entity<Post>()
                    .Property(e => e.LastUpdated)
                    .HasConversion(
                        v => v,
                        v => new DateTime(v.Ticks, DateTimeKind.Utc));
-->
[!code-csharp[ConfigurePreserveDateTimeKind2](../../../samples/core/Modeling/ValueConversions/PreserveDateTimeKind.cs?name=ConfigurePreserveDateTimeKind2)]

If a mix of local and UTC values are being set in entity instances, then the converter can be used to convert appropriately before inserting. For example:

<!--
                modelBuilder.Entity<Post>()
                    .Property(e => e.LastUpdated)
                    .HasConversion(
                        v => v.ToUniversalTime(),
                        v => new DateTime(v.Ticks, DateTimeKind.Utc));
-->
[!code-csharp[ConfigurePreserveDateTimeKind3](../../../samples/core/Modeling/ValueConversions/PreserveDateTimeKind.cs?name=ConfigurePreserveDateTimeKind3)]

> [!NOTE]
> Carefully consider unifying all database access code to use UTC time all the time, only dealing with local time when presenting data to users.

### Use case-insensitive string keys

Some databases, including SQL Server, perform case-insensitive string comparisons by default. .NET, on the other hand, performs case-sensitive string comparisons by default. This means that a foreign key value like "DotNet" will match the primary key value "dotnet" on SQL Server, but will not match it in EF Core. A value comparer for keys can be used to force EF Core into case-insensitive string comparisons like in the database. For example, consider a blog/posts model with string keys:

<!--
        public class Blog
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public ICollection<Post> Posts { get; set; }
        }

        public class Post
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public string BlogId { get; set; }
            public Blog Blog { get; set; }
        }
-->
[!code-csharp[CaseInsensitiveStringsModel](../../../samples/core/Modeling/ValueConversions/CaseInsensitiveStrings.cs?name=CaseInsensitiveStringsModel)]

This will not work as expected if some of the `Post.BlogId` values have different casing. The errors caused by this will depend on what the application is doing, but typically involve graphs of objects that are not [fixed-up](xref:core/change-tracking/relationship-changes) correctly, and/or updates that fail because the FK value is wrong. A value comparer can be used to correct this:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var comparer = new ValueComparer<string>(
                    (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                    v => v.ToUpper().GetHashCode(),
                    v => v);

                modelBuilder.Entity<Blog>()
                    .Property(e => e.Id)
                    .Metadata.SetValueComparer(comparer);

                modelBuilder.Entity<Post>(
                    b =>
                        {
                            b.Property(e => e.Id).Metadata.SetValueComparer(comparer);
                            b.Property(e => e.BlogId).Metadata.SetValueComparer(comparer);
                        });
            }
-->
[!code-csharp[ConfigureCaseInsensitiveStrings](../../../samples/core/Modeling/ValueConversions/CaseInsensitiveStrings.cs?name=ConfigureCaseInsensitiveStrings)]

> [!NOTE]
> .NET string comparisons and database string comparisons can differ in more than just case sensitivity. This pattern works for simple ASCII keys, but may fail for keys with any kind of culture-specific characters. See [Collations and Case Sensitivity](xref:core/miscellaneous/collations-and-case-sensitivity) for more information.

### Handle fixed-length database strings

The previous example did not need a value converter. However, a converter can be useful for fixed-length database string types like `char(20)` or `nchar(20)`. Fixed-length strings are padded to their full length whenever a value is inserted into the database. This means that a key value of "`dotnet`" will be read back from the database as "`dotnet..............`", where `.` represents a space character. This will then not compare correctly with key values that are not padded.

A value converter can be used to trim the padding when reading key values. This can be combined with the value comparer in the previous example to compare fixed length case-insensitive ASCII keys correctly. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var converter = new ValueConverter<string, string>(
                    v => v,
                    v => v.Trim());
                
                var comparer = new ValueComparer<string>(
                    (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                    v => v.ToUpper().GetHashCode(),
                    v => v);

                modelBuilder.Entity<Blog>()
                    .Property(e => e.Id)
                    .HasColumnType("char(20)")
                    .HasConversion(converter, comparer);

                modelBuilder.Entity<Post>(
                    b =>
                        {
                            b.Property(e => e.Id).HasColumnType("char(20)").HasConversion(converter, comparer);
                            b.Property(e => e.BlogId).HasColumnType("char(20)").HasConversion(converter, comparer);
                        });
            }
-->
[!code-csharp[ConfigureFixedLengthStrings](../../../samples/core/Modeling/ValueConversions/FixedLengthStrings.cs?name=ConfigureFixedLengthStrings)]

### Encrypt property values

Value converters can be used to encrypt property values before sending them to the database, and then decrypt them on the way out. For example, using string reversal as a substitute for a real encryption algorithm:

<!--
                modelBuilder.Entity<User>().Property(e => e.Password).HasConversion(
                    v => new string(v.Reverse().ToArray()),
                    v => new string(v.Reverse().ToArray()));
-->
[!code-csharp[ConfigureEncryptPropertyValues](../../../samples/core/Modeling/ValueConversions/EncryptPropertyValues.cs?name=ConfigureEncryptPropertyValues)]

> [!NOTE]
> There is currently no way to get a reference to the current DbContext, or other session state, from within a value converter. This limits the kinds of encryption that can be used. Vote for [GitHub issue #11597](https://github.com/dotnet/efcore/issues/12205) to have this limitation removed.

> [!WARNING]
> Make sure to understand all the implications if you roll your own encryption to protect sensitive data. Consider instead using pre-built encryption mechanisms, such as [Always Encrypted](/sql/relational-databases/security/encryption/always-encrypted-database-engine) on SQL Server.

## Limitations

There are a few known current limitations of the value conversion system:

* As noted above, `null` cannot be converted. Please vote (👍) for [GitHub issue #13850](https://github.com/dotnet/efcore/issues/13850) if this is something you need.
* It isn't possible to query into value-converted properties, e.g. reference members on the value-converted .NET type in your LINQ queries. Please vote (👍) for [GitHub issue #10434](https://github.com/dotnet/efcore/issues/10434) if this is something you need - but considering using a [JSON column](xref:core/what-is-new/ef-core-7.0/whatsnew#json-columns) instead.
* There is currently no way to spread a conversion of one property to multiple columns or vice-versa. Please vote (👍) for [GitHub issue #13947](https://github.com/dotnet/efcore/issues/13947) if this is something you need.
* Value generation is not supported for most keys mapped through value converters. Please vote (👍) for [GitHub issue #11597](https://github.com/dotnet/efcore/issues/11597) if this is something you need.
* Value conversions cannot reference the current DbContext instance. Please vote (👍) for [GitHub issue #12205](https://github.com/dotnet/efcore/issues/12205) if this is something you need.
* Parameters using value-converted types cannot currently be used in raw SQL APIs. Please vote (👍) for [GitHub issue #27534](https://github.com/dotnet/efcore/issues/27354) if this is something you need.

Removal of these limitations is being considered for future releases.
