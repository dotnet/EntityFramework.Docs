---
title: Function Mappings - Azure Cosmos DB Provider - EF Core
description: Function Mappings of the Azure Cosmos DB EF Core Provider
author: bricelam
ms.date: 7/26/2023
uid: core/providers/cosmos/functions
---
# Function Mappings of the Azure Cosmos DB EF Core Provider

This page shows which .NET members are translated into which SQL functions when using the Azure Cosmos DB provider.

## Date and time functions

.NET                  | SQL
--------------------- | ---
DateTime.UtcNow       | GetCurrentDateTime()
DateTimeOffset.UtcNow | GetCurrentDateTime()

## Numeric functions

.NET                       | SQL               | Added in
-------------------------- | ----------------- | --------
double.DegreesToRadians(x) | RADIANS(@x)       | EF Core 8.0
double.RadiansToDegrees(x) | DEGREES(@x)       | EF Core 8.0
EF.Functions.Random()      | RAND()
Math.Abs(value)            | ABS(@value)
Math.Acos(d)               | ACOS(@d)
Math.Asin(d)               | ASIN(@d)
Math.Atan(d)               | ATAN(@d)
Math.Atan2(y, x)           | ATN2(@y, @x)
Math.Ceiling(d)            | CEILING(@d)
Math.Cos(d)                | COS(@d)
Math.Exp(d)                | EXP(@d)
Math.Floor(d)              | FLOOR(@d)
Math.Log(a, newBase)       | LOG(@a, @newBase)
Math.Log(d)                | LOG(@d)
Math.Log10(d)              | LOG10(@d)
Math.Pow(x, y)             | POWER(@x, @y)
Math.Round(d)              | ROUND(@d)
Math.Sign(value)           | SIGN(@value)
Math.Sin(a)                | SIN(@a)
Math.Sqrt(d)               | SQRT(@d)
Math.Tan(a)                | TAN(@a)
Math.Truncate(d)           | TRUNC(@d)

> [!TIP]
> In addition to the methods listed here, corresponding [generic math](/dotnet/standard/generics/math) implementations
> and [MathF](/dotnet/api/system.mathf) methods are also translated. For example, `Math.Sin`, `MathF.Sin`, `double.Sin`,
> and `float.Sin` all map to the `SIN` function in SQL.

## String functions

.NET                                                          | SQL                                                        | Added in
------------------------------------------------------------- | ---------------------------------------------------------- | --------
Regex.IsMatch(input, pattern)                                 | RegexMatch(@pattern, @input)                               | EF Core 7.0
Regex.IsMatch(input, pattern, options)                        | RegexMatch(@input, @pattern, @options)                     | EF Core 7.0
string.Concat(str0, str1)                                     | @str0 + @str1
string.Equals(a, b, StringComparison.Ordinal)                 | STRINGEQUALS(@a, @b)
string.Equals(a, b, StringComparison.OrdinalIgnoreCase)       | STRINGEQUALS(@a, @b, true)
stringValue.Contains(value)                                   | CONTAINS(@stringValue, @value)
stringValue.EndsWith(value)                                   | ENDSWITH(@stringValue, @value)
stringValue.Equals(value, StringComparison.Ordinal)           | STRINGEQUALS(@stringValue, @value)
stringValue.Equals(value, StringComparison.OrdinalIgnoreCase) | STRINGEQUALS(@stringValue, @value, true)
stringValue.FirstOrDefault()                                  | LEFT(@stringValue, 1)
stringValue.IndexOf(value)                                    | INDEX_OF(@stringValue, @value)
stringValue.IndexOf(value, startIndex)                        | INDEX_OF(@stringValue, @value, @startIndex)
stringValue.LastOrDefault()                                   | RIGHT(@stringValue, 1)
stringValue.Length                                            | LENGTH(@stringValue)
stringValue.Replace(oldValue, newValue)                       | REPLACE(@stringValue, @oldValue, @newValue)
stringValue.StartsWith(value)                                 | STARTSWITH(@stringValue, @value)
stringValue.Substring(startIndex)                             | SUBSTRING(@stringValue, @startIndex, LENGTH(@stringValue))
stringValue.Substring(startIndex, length)                     | SUBSTRING(@stringValue, @startIndex, @length)
stringValue.ToLower()                                         | LOWER(@stringValue)
stringValue.ToUpper()                                         | UPPER(@stringValue)
stringValue.Trim()                                            | TRIM(@stringValue)
stringValue.TrimEnd()                                         | RTRIM(@stringValue)
stringValue.TrimStart()                                       | LTRIM(@stringValue)

## Miscellaneous functions

.NET                      | SQL
--------------------------|----
collection.Contains(item) | @item IN @collection
