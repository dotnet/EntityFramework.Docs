---
title: Function Mappings - Azure Cosmos DB Provider - EF Core
description: Function Mappings of the Azure Cosmos DB EF Core Provider
author: bricelam
ms.date: 10/11/2021
uid: core/providers/cosmos/functions
---
# Function Mappings of the Azure Cosmos DB EF Core Provider

This page shows which .NET members are translated into which SQL functions when using the Azure Cosmos DB provider.

## Date and time functions

.NET                  | SQL                  | Added in
--------------------- | -------------------- | --------
DateTime.UtcNow       | GetCurrentDateTime() | EF Core 6.0
DateTimeOffset.UtcNow | GetCurrentDateTime() | EF Core 6.0

## Numeric functions

.NET                  | SQL               | Added in
--------------------- | ----------------- | --------
EF.Functions.Random() | RAND()            | EF Core 6.0
Math.Abs(value)       | ABS(@value)       | EF Core 6.0
Math.Acos(d)          | ACOS(@d)          | EF Core 6.0
Math.Asin(d)          | ASIN(@d)          | EF Core 6.0
Math.Atan(d)          | ATAN(@d)          | EF Core 6.0
Math.Atan2(y, x)      | ATN2(@y, @x)      | EF Core 6.0
Math.Ceiling(d)       | CEILING(@d)       | EF Core 6.0
Math.Cos(d)           | COS(@d)           | EF Core 6.0
Math.Exp(d)           | EXP(@d)           | EF Core 6.0
Math.Floor(d)         | FLOOR(@d)         | EF Core 6.0
Math.Log(a, newBase)  | LOG(@a, @newBase) | EF Core 6.0
Math.Log(d)           | LOG(@d)           | EF Core 6.0
Math.Log10(d)         | LOG10(@d)         | EF Core 6.0
Math.Pow(x, y)        | POWER(@x, @y)     | EF Core 6.0
Math.Round(d)         | ROUND(@d)         | EF Core 6.0
Math.Sign(value)      | SIGN(@value)      | EF Core 6.0
Math.Sin(a)           | SIN(@a)           | EF Core 6.0
Math.Sqrt(d)          | SQRT(@d)          | EF Core 6.0
Math.Tan(a)           | TAN(@a)           | EF Core 6.0
Math.Truncate(d)      | TRUNC(@d)         | EF Core 6.0
MathF.Abs(x)          | ABS(@x)           | EF Core 6.0
MathF.Acos(x)         | ACOS(@x)          | EF Core 6.0
MathF.Asin(x)         | ASIN(@x)          | EF Core 6.0
MathF.Atan(x)         | ATAN(@x)          | EF Core 6.0
MathF.Atan2(y, x)     | ATN2(@y, @x)      | EF Core 6.0
MathF.Ceiling(x)      | CEILING(@x)       | EF Core 6.0
MathF.Cos(x)          | COS(@x)           | EF Core 6.0
MathF.Exp(x)          | EXP(@x)           | EF Core 6.0
MathF.Floor(x)        | FLOOR(@x)         | EF Core 6.0
MathF.Log(x, y)       | LOG(@x, @y)       | EF Core 6.0
MathF.Log(x)          | LOG(@x)           | EF Core 6.0
MathF.Log10(x)        | LOG10(@x)         | EF Core 6.0
MathF.Pow(x, y)       | POWER(@x, @y)     | EF Core 6.0
MathF.Round(x)        | ROUND(@x)         | EF Core 6.0
MathF.Sign(x)         | SIGN(@x)          | EF Core 6.0
MathF.Sin(x)          | SIN(@x)           | EF Core 6.0
MathF.Sqrt(x)         | SQRT(@x)          | EF Core 6.0
MathF.Tan(x)          | TAN(@x)           | EF Core 6.0
MathF.Truncate(x)     | TRUNC(@x)         | EF Core 6.0

## String functions

.NET                                                          | SQL                                                        | Added in
------------------------------------------------------------- | ---------------------------------------------------------- | --------
string.Concat(str0, str1)                                     | @str0 + @str1                                              | EF Core 6.0
string.Equals(a, b, StringComparison.Ordinal)                 | STRINGEQUALS(@a, @b)                                       | EF Core 6.0
string.Equals(a, b, StringComparison.OrdinalIgnoreCase)       | STRINGEQUALS(@a, @b, true)                                 | EF Core 6.0
stringValue.Contains(value)                                   | CONTAINS(@stringValue, @value)                             | EF Core 5.0
stringValue.EndsWith(value)                                   | ENDSWITH(@stringValue, @value)                             | EF Core 5.0
stringValue.Equals(value, StringComparison.Ordinal)           | STRINGEQUALS(@stringValue, @value)                         | EF Core 6.0
stringValue.Equals(value, StringComparison.OrdinalIgnoreCase) | STRINGEQUALS(@stringValue, @value, true)                   | EF Core 6.0
stringValue.FirstOrDefault()                                  | LEFT(@stringValue, 1)                                      | EF Core 5.0
stringValue.IndexOf(value)                                    | INDEX_OF(@stringValue, @value)                             | EF Core 6.0
stringValue.IndexOf(value, startIndex)                        | INDEX_OF(@stringValue, @value, @startIndex)                | EF Core 6.0
stringValue.LastOrDefault()                                   | RIGHT(@stringValue, 1)                                     | EF Core 5.0
stringValue.Length                                            | LENGTH(@stringValue)                                       | EF Core 6.0
stringValue.Replace(oldValue, newValue)                       | REPLACE(@stringValue, @oldValue, @newValue)                | EF Core 6.0
stringValue.StartsWith(value)                                 | STARTSWITH(@stringValue, @value)                           | EF Core 5.0
stringValue.Substring(startIndex)                             | SUBSTRING(@stringValue, @startIndex, LENGTH(@stringValue)) | EF Core 6.0
stringValue.Substring(startIndex, length)                     | SUBSTRING(@stringValue, @startIndex, @length)              | EF Core 6.0
stringValue.ToLower()                                         | LOWER(@stringValue)                                        | EF Core 6.0
stringValue.ToUpper()                                         | UPPER(@stringValue)                                        | EF Core 6.0
stringValue.Trim()                                            | TRIM(@stringValue)                                         | EF Core 6.0
stringValue.TrimEnd()                                         | RTRIM(@stringValue)                                        | EF Core 6.0
stringValue.TrimStart()                                       | LTRIM(@stringValue)                                        | EF Core 6.0

## Miscellaneous functions

.NET                      | SQL
------------------------- | ---
collection.Contains(item) | @item IN @collection
