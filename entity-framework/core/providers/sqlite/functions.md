---
title: Function Mappings - SQLite Database Provider - EF Core
description: Function Mappings of the SQLite EF Core database provider
author: SamMonoRT
ms.date: 7/26/2023
uid: core/providers/sqlite/functions
---
# Function Mappings of the SQLite EF Core Provider

This page shows which .NET members are translated into which SQL functions when using the SQLite provider.

## Aggregate functions

.NET                                                  | SQL                                | Added in
----------------------------------------------------- | ---------------------------------- | --------
group.Average(x => x.Property)                        | AVG(Property)
group.Average(x => x.DecimalProperty)                 | ef_avg(DecimalProperty)            | EF Core 9.0
group.Count()                                         | COUNT(*)
group.LongCount()                                     | COUNT(*)
group.Max(x => x.Property)                            | MAX(Property)
group.Min(x => x.Property)                            | MIN(Property)
group.Sum(x => x.Property)                            | SUM(Property)
group.Sum(x => x.DecimalProperty)                     | ef_sum(DecimalProperty)            | EF Core 9.0
string.Concat(group.Select(x => x.Property))          | group_concat(Property, '')         | EF Core 7.0
string.Join(separator, group.Select(x => x.Property)) | group_concat(Property, @separator) | EF Core 7.0

## Binary functions

.NET                                           | SQL                                  | Added in
---------------------------------------------- | ------------------------------------ | --------
bytes.Contains(value)                          | instr(@bytes, char(@value)) > 0
bytes.Length                                   | length(@bytes)
bytes.SequenceEqual(second)                    | @bytes = @second
EF.Functions.Hex(bytes)                        | hex(@bytes)
EF.Functions.Substr(bytes, startIndex)         | substr(@bytes, @startIndex)
EF.Functions.Substr(bytes, startIndex, length) | substr(@bytes, @startIndex, @length)
EF.Functions.Unhex(value)                      | unhex(@value)                        | EF Core 8.0
EF.Functions.Unhex(value, ignoreChars)         | unhex(@value, @ignoreChars)          | EF Core 8.0

## Conversion functions

.NET                      | SQL
------------------------- | ---
boolValue.ToString()      | CAST(@boolValue AS TEXT)
byteValue.ToString()      | CAST(@byteValue AS TEXT)
bytes.ToString()          | CAST(@bytes AS TEXT)
charValue.ToString()      | CAST(@charValue AS TEXT)
dateTime.ToString()       | CAST(@dateTime AS TEXT)
dateTimeOffset.ToString() | CAST(@dateTimeOffset AS TEXT)
decimalValue.ToString()   | CAST(@decimalValue AS TEXT)
doubleValue.ToString()    | CAST(@doubleValue AS TEXT)
floatValue.ToString()     | CAST(@floatValue AS TEXT)
guid.ToString()           | CAST(@guid AS TEXT)
intValue.ToString()       | CAST(@intValue AS TEXT)
longValue.ToString()      | CAST(@longValue AS TEXT)
sbyteValue.ToString()     | CAST(@sbyteValue AS TEXT)
shortValue.ToString()     | CAST(@shortValue AS TEXT)
timeSpan.ToString()       | CAST(@timeSpan AS TEXT)
uintValue.ToString()      | CAST(@uintValue AS TEXT)
ushortValue.ToString()    | CAST(@ushortValue AS TEXT)

## Date and time functions

.NET                            | SQL                                                                      | Added in
------------------------------- | ------------------------------------------------------------------------ | --------
dateOnly.AddDays(value)         | date(@dateOnly, @value \|\| ' days')
dateOnly.AddMonths(months)      | date(@dateOnly, @months \|\| ' months')
dateOnly.AddYears(value)        | date(@dateOnly, @value \|\| ' years')
dateOnly.Day                    | strftime('%d', @dateOnly)
dateOnly.DayOfWeek              | strftime('%w', @dateOnly)
dateOnly.DayOfYear              | strftime('%j', @dateOnly)
DateOnly.FromDateTime(dateTime) | date(@dateTime)                                                          | EF Core 8.0
dateOnly.Month                  | strftime('%m', @dateOnly)
dateOnly.Year                   | strftime('%Y', @dateOnly)
DateTime.Now                    | datetime('now', 'localtime')
DateTime.Today                  | datetime('now', 'localtime', 'start of day')
DateTime.UtcNow                 | datetime('now')
dateTime.AddDays(value)         | datetime(@dateTime, @value \|\| ' days')
dateTime.AddHours(value)        | datetime(@dateTime, @d \|\| ' hours')
dateTime.AddMilliseconds(value) | datetime(@dateTime, (@value / 1000.0) \|\| ' seconds')
dateTime.AddMinutes(value)      | datetime(@dateTime, @value \|\| ' minutes')
dateTime.AddMonths(months)      | datetime(@dateTime, @months \|\| ' months')
dateTime.AddSeconds(value)      | datetime(@dateTime, @value \|\| ' seconds')
dateTime.AddTicks(value)        | datetime(@dateTime, (@value / 10000000.0) \|\| ' seconds')
dateTime.AddYears(value)        | datetime(@dateTime, @value \|\| ' years')
dateTime.Date                   | datetime(@dateTime, 'start of day')
dateTime.Day                    | strftime('%d', @dateTime)
dateTime.DayOfWeek              | strftime('%w', @dateTime)
dateTime.DayOfYear              | strftime('%j', @dateTime)
dateTime.Hour                   | strftime('%H', @dateTime)
dateTime.Millisecond            | (strftime('%f', @dateTime) * 1000) % 1000
dateTime.Minute                 | strftime('%M', @dateTime)
dateTime.Month                  | strftime('%m', @dateTime)
dateTime.Second                 | strftime('%S', @dateTime)
dateTime.Ticks                  | (julianday(@dateTime) - julianday('0001-01-01 00:00:00')) * 864000000000
dateTime.TimeOfDay              | time(@dateTime)
dateTime.Year                   | strftime('%Y', @dateTime)

> [!NOTE]
> Some SQL has been simplified for illustration purposes. The actual SQL is more complex to handle a wider range of values.

## Numeric functions

.NET                             | SQL                                  | Added in
-------------------------------- | ------------------------------------ | --------
-decimalValue                    | ef_negate(@decimalValue)
decimalValue - d                 | ef_add(@decimalValue, ef_negate(@d))
decimalValue * d                 | ef_multiply(@decimalValue, @d)
decimalValue / d                 | ef_divide(@decimalValue, @d)
decimalValue % d                 | ef_mod(@decimalValue, @d)
decimalValue + d                 | ef_add(@decimalValue, @d)
decimalValue < d                 | ef_compare(@decimalValue, @d) < 0
decimalValue <= d                | ef_compare(@decimalValue, @d) <= 0
decimalValue > d                 | ef_compare(@decimalValue, @d) > 0
decimalValue >= d                | ef_compare(@decimalValue, @d) >= 0
double.DegreesToRadians(degrees) | radians(@degrees)                     | EF Core 8.0
double.RadiansToDegrees(radians) | degrees(@dradians)                    | EF Core 8.0
doubleValue % d                  | mod(@doubleValue, @d)
EF.Functions.Random()            | abs(random() / 9223372036854780000.0)
Math.Abs(value)                  | abs(@value)
Math.Acos(value)                 | acos(@value)                          | EF Core 8.0
Math.Acosh(d)                    | acosh(@d)                             | EF Core 8.0
Math.Asin(d)                     | asin(@d)                              | EF Core 8.0
Math.Asinh(d)                    | asinh(@d)                             | EF Core 8.0
Math.Atan(d)                     | atan(@d)                              | EF Core 8.0
Math.Atan2(y, x)                 | atan2(@y, @x)                         | EF Core 8.0
Math.Atanh(d)                    | atanh(@d)                             | EF Core 8.0
Math.Ceiling(d)                  | ceiling(@d)                           | EF Core 8.0
Math.Cos(d)                      | cos(@d)                               | EF Core 8.0
Math.Cosh(value)                 | cosh(@value)                          | EF Core 8.0
Math.Exp(d)                      | exp(@d)                               | EF Core 8.0
Math.Floor(d)                    | floor(@d)                             | EF Core 8.0
Math.Log(d)                      | ln(@d)                                | EF Core 8.0
Math.Log(a, newBase)             | log(@newBase, @a)                     | EF Core 8.0
Math.Log2(x)                     | log2(@x)                              | EF Core 8.0
Math.Log10(d)                    | log10(@d)                             | EF Core 8.0
Math.Max(val1, val2)             | max(@val1, @val2)
Math.Min(val1, val2)             | min(@val1, @val2)
Math.Pow(x, y)                   | pow(@x, @y)                           | EF Core 8.0
Math.Round(d)                    | round(@d)
Math.Round(d, digits)            | round(@d, @digits)
Math.Sign(d)                     | sign(@d)                              | EF Core 8.0
Math.Sin(a)                      | sin(@a)                               | EF Core 8.0
Math.Sinh(value)                 | sinh(@value)                          | EF Core 8.0
Math.Sqrt(d)                     | sqrt(@d)                              | EF Core 8.0
Math.Tan(a)                      | tan(@a)                               | EF Core 8.0
Math.Tanh(value)                 | tanh(@value)                          | EF Core 8.0
Math.Truncate(d)                 | trunc(@d)                             | EF Core 8.0

> [!TIP]
> In addition to the methods listed here, corresponding [generic math](/dotnet/standard/generics/math) implementations
> and <xref:System.MathF> methods are also translated. For example, `Math.Sin`, `MathF.Sin`, `double.Sin`,
> and `float.Sin` all map to the `sin` function in SQL.

> [!TIP]
> SQL functions prefixed with `ef_` are created by EF Core.

## String functions

.NET                                                         | SQL
------------------------------------------------------------ | ---
char.ToLower(c)                                              | lower(@c)
char.ToUpper(c)                                              | upper(@c)
EF.Functions.Collate(operand, collation)                     | @operand COLLATE @collation
EF.Functions.Glob(matchExpression, pattern)                  | @matchExpression GLOB @pattern
EF.Functions.Like(matchExpression, pattern)                  | @matchExpression LIKE @pattern
EF.Functions.Like(matchExpression, pattern, escapeCharacter) | @matchExpression LIKE @pattern ESCAPE @escapeCharacter
Regex.IsMatch(input, pattern)                                | @input REGEXP @pattern
string.Compare(strA, strB)                                   | CASE WHEN @strA = @strB THEN 0 ... END
string.Concat(str0, str1)                                    | @str0 \|\| @str1
string.IsNullOrEmpty(value)                                  | @value IS NULL OR @value = ''
string.IsNullOrWhiteSpace(value)                             | @value IS NULL OR trim(@value) = ''
stringValue.CompareTo(strB)                                  | CASE WHEN @stringValue = @strB THEN 0 ... END
stringValue.Contains(value)                                  | instr(@stringValue, @value) > 0
stringValue.EndsWith(value)                                  | @stringValue LIKE '%' \|\| @value
stringValue.FirstOrDefault()                                 | substr(@stringValue, 1, 1)
stringValue.IndexOf(value)                                   | instr(@stringValue, @value) - 1
stringValue.LastOrDefault()                                  | substr(@stringValue, length(@stringValue), 1)
stringValue.Length                                           | length(@stringValue)
stringValue.Replace(oldValue, newValue)                      | replace(@stringValue, @oldValue, @newValue)
stringValue.StartsWith(value)                                | @stringValue LIKE @value \|\| '%'
stringValue.Substring(startIndex)                            | substr(@stringValue, @startIndex + 1)
stringValue.Substring(startIndex, length)                    | substr(@stringValue, @startIndex + 1, @length)
stringValue.ToLower()                                        | lower(@stringValue)
stringValue.ToUpper()                                        | upper(@stringValue)
stringValue.Trim()                                           | trim(@stringValue)
stringValue.Trim(trimChar)                                   | trim(@stringValue, @trimChar)
stringValue.TrimEnd()                                        | rtrim(@stringValue)
stringValue.TrimEnd(trimChar)                                | rtrim(@stringValue, @trimChar)
stringValue.TrimStart()                                      | ltrim(@stringValue)
stringValue.TrimStart(trimChar)                              | ltrim(@stringValue, @trimChar)

> [!NOTE]
> Some SQL has been simplified for illustration purposes. The actual SQL is more complex to handle a wider range of values.

## Miscellaneous functions

.NET                                     | SQL
---------------------------------------- | ---
collection.Contains(item)                | @item IN @collection
enumValue.HasFlag(flag)                  | @enumValue & @flag = @flag
nullable.GetValueOrDefault()             | coalesce(@nullable, 0)
nullable.GetValueOrDefault(defaultValue) | coalesce(@nullable, @defaultValue)

## See also

* [Spatial Function Mappings](xref:core/providers/sqlite/spatial#spatial-function-mappings)
