---
title: Function Mappings - SQLite Database Provider - EF Core
description: Function Mappings of the SQLite EF Core database provider
author: bricelam
ms.date: 11/15/2021
uid: core/providers/sqlite/functions
---
# Function Mappings of the SQLite EF Core Provider

This page shows which .NET members are translated into which SQL functions when using the SQLite provider.

## Aggregate functions

.NET                                                  | SQL                                | Added in
----------------------------------------------------- | ---------------------------------- | --------
group.Average(x => x.Property)                        | AVG(Property)
group.Count()                                         | COUNT(*)
group.LongCount()                                     | COUNT(*)
group.Max(x => x.Property)                            | MAX(Property)
group.Min(x => x.Property)                            | MIN(Property)
group.Sum(x => x.Property)                            | SUM(Property)
string.Concat(group.Select(x => x.Property))          | group_concat(Property, '')         | EF Core 7.0
string.Join(separator, group.Select(x => x.Property)) | group_concat(Property, @separator) | EF Core 7.0

## Binary functions

.NET                                           | SQL
---------------------------------------------- | ---
bytes.Contains(value)                          | instr(@bytes, char(@value)) > 0
bytes.Length                                   | length(@bytes)
bytes.SequenceEqual(second)                    | @bytes = @second
EF.Functions.Hex(bytes)                        | hex(@bytes)
EF.Functions.Substr(bytes, startIndex)         | substr(@bytes, @startIndex)
EF.Functions.Substr(bytes, startIndex, length) | substr(@bytes, @startIndex, @length)

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

.NET                            | SQL
------------------------------- | ---
dateOnly.AddDays(value)         | date(@dateOnly, @value \|\| ' days')
dateOnly.AddMonths(months)      | date(@dateOnly, @months \|\| ' months')
dateOnly.AddYears(value)        | date(@dateOnly, @value \|\| ' years')
dateOnly.Day                    | strftime('%d', @dateOnly)
dateOnly.DayOfWeek              | strftime('%w', @dateOnly)
dateOnly.DayOfYear              | strftime('%j', @dateOnly)
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

.NET                   | SQL
---------------------- | ---
-decimalValue          | ef_negate(@decimalValue)
decimalValue - d       | ef_add(@decimalValue, ef_negate(@d))
decimalValue * d       | ef_multiply(@decimalValue, @d)
decimalValue / d       | ef_divide(@decimalValue, @d)
decimalValue % d       | ef_mod(@decimalValue, @d)
decimalValue + d       | ef_add(@decimalValue, @d)
decimalValue < d       | ef_compare(@decimalValue, @d) < 0
decimalValue <= d      | ef_compare(@decimalValue, @d) <= 0
decimalValue > d       | ef_compare(@decimalValue, @d) > 0
decimalValue >= d      | ef_compare(@decimalValue, @d) >= 0
doubleValue % d        | ef_mod(@doubleValue, @d)
EF.Functions.Random()  | abs(random() / 9223372036854780000.0)
floatValue % d         | ef_mod(@floatValue, @d)
Math.Abs(value)        | abs(@value)
Math.Max(val1, val2)   | max(@val1, @val2)
Math.Min(val1, val2)   | min(@val1, @val2)
Math.Round(d)          | round(@d)
Math.Round(d, digits)  | round(@d, @digits)
MathF.Abs(x)           | abs(@x)
MathF.Max(x, y)        | max(@x, @y)
MathF.Min(x, y)        | min(@x, @y)
MathF.Round(x)         | round(@x)
MathF.Round(x, digits) | round(@x, @digits)

> [!TIP]
> SQL functions prefixed with *ef* are created by EF Core.

## String functions

.NET                                                         | SQL
------------------------------------------------------------ | ---
char.ToLower(c)                                              | lower(@c)
char.ToUpper(c)                                              | upper(@c)
EF.Functions.Collate(operand, collation)                     | @operand COLLATE @collation
EF.Functions.Glob(matchExpression, pattern)                  | glob(@pattern, @matchExpression)
EF.Functions.Like(matchExpression, pattern)                  | @matchExpression LIKE @pattern
EF.Functions.Like(matchExpression, pattern, escapeCharacter) | @matchExpression LIKE @pattern ESCAPE @escapeCharacter
Regex.IsMatch(input, pattern)                                | regexp(@pattern, @input)
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
