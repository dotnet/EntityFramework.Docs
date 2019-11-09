---
title: Value Conversions - EF Core
author: ajcvickers
ms.date: 02/19/2018
ms.assetid: 3154BF3C-1749-4C60-8D51-AE86773AA116
uid: core/modeling/value-conversions
---
# Value Conversions

> [!NOTE]  
> This feature is new in EF Core 2.1.

Value converters allow property values to be converted when reading from or writing to the database. This conversion can be from one value to another of the same type (for example, encrypting strings) or from a value of one type to a value of another type (for example, converting enum values to and from strings in the database.)

## Fundamentals

Value converters are specified in terms of a `ModelClrType` and a `ProviderClrType`. The model type is the .NET type of the property in the entity type. The provider type is the .NET type understood by the database provider. For example, to save enums as strings in the database, the model type is the type of the enum, and the provider type is `String`. These two types can be the same.

Conversions are defined using two `Func` expression trees: one from `ModelClrType` to `ProviderClrType` and the other from `ProviderClrType` to `ModelClrType`. Expression trees are used so that they can be compiled into the database access code for efficient conversions. For complex conversions, the expression tree may be a simple call to a method that performs the conversion.

## Configuring a value converter

Value conversions are defined on properties in the `OnModelCreating` of your `DbContext`. For example, consider an enum and entity type defined as:

``` csharp
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
```

Then conversions can be defined in `OnModelCreating` to store the enum values as strings (for example, "Donkey", "Mule", ...) in the database:

``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .Entity<Rider>()
        .Property(e => e.Mount)
        .HasConversion(
            v => v.ToString(),
            v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));
}
```

> [!NOTE]  
> A `null` value will never be passed to a value converter. This makes the implementation of conversions easier and allows them to be shared amongst nullable and non-nullable properties.

## The ValueConverter class

Calling `HasConversion` as shown above will create a `ValueConverter` instance and set it on the property. The `ValueConverter` can instead be created explicitly. For example:

``` csharp
var converter = new ValueConverter<EquineBeast, string>(
    v => v.ToString(),
    v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

modelBuilder
    .Entity<Rider>()
    .Property(e => e.Mount)
    .HasConversion(converter);
```

This can be useful when multiple properties use the same conversion.

> [!NOTE]  
> There is currently no way to specify in one place that every property of a given type must use the same value converter. This feature will be considered for a future release.

## Built-in converters

EF Core ships with a set of pre-defined `ValueConverter` classes, found in the `Microsoft.EntityFrameworkCore.Storage.ValueConversion` namespace. These are:

* `BoolToZeroOneConverter` - Bool to zero and one
* `BoolToStringConverter` - Bool to strings such as "Y" and "N"
* `BoolToTwoValuesConverter` - Bool to any two values
* `BytesToStringConverter` - Byte array to Base64-encoded string
* `CastingConverter` - Conversions that require only a type cast
* `CharToStringConverter` - Char to single character string
* `DateTimeOffsetToBinaryConverter` - DateTimeOffset to binary-encoded 64-bit value
* `DateTimeOffsetToBytesConverter` - DateTimeOffset to byte array
* `DateTimeOffsetToStringConverter` - DateTimeOffset to string
* `DateTimeToBinaryConverter` - DateTime to 64-bit value including DateTimeKind
* `DateTimeToStringConverter` - DateTime to string
* `DateTimeToTicksConverter` - DateTime to ticks
* `EnumToNumberConverter` - Enum to underlying number
* `EnumToStringConverter` - Enum to string
* `GuidToBytesConverter` - Guid to byte array
* `GuidToStringConverter` - Guid to string
* `NumberToBytesConverter` - Any numerical value to byte array
* `NumberToStringConverter` - Any numerical value to string
* `StringToBytesConverter` - String to UTF8 bytes
* `TimeSpanToStringConverter` - TimeSpan to string
* `TimeSpanToTicksConverter` - TimeSpan to ticks

Notice that `EnumToStringConverter` is included in this list. This means that there is no need to specify the conversion explicitly, as shown above. Instead, just use the built-in converter:

``` csharp
var converter = new EnumToStringConverter<EquineBeast>();

modelBuilder
    .Entity<Rider>()
    .Property(e => e.Mount)
    .HasConversion(converter);
```

Note that all the built-in converters are stateless and so a single instance can be safely shared by multiple properties.

## Pre-defined conversions

For common conversions for which a built-in converter exists there is no need to specify the converter explicitly. Instead, just configure which provider type should be used and EF will automatically use the appropriate built-in converter. Enum to string conversions are used as an example above, but EF will actually do this automatically if the provider type is configured:

``` csharp
modelBuilder
    .Entity<Rider>()
    .Property(e => e.Mount)
    .HasConversion<string>();
```

The same thing can be achieved by explicitly specifying the column type. For example, if the entity type is defined like so:

``` csharp
public class Rider
{
    public int Id { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public EquineBeast Mount { get; set; }
}
```

Then the enum values will be saved as strings in the database without any further configuration in `OnModelCreating`.

## Limitations

There are a few known current limitations of the value conversion system:

* As noted above, `null` cannot be converted.
* There is currently no way to spread a conversion of one property to multiple columns or vice-versa.
* Use of value conversions may impact the ability of EF Core to translate expressions to SQL. A warning will be logged for such cases.
Removal of these limitations is being considered for a future release.
