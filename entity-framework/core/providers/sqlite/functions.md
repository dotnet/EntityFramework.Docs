---
title: Function Mappings - SQLite Database Provider - EF Core
description: Function Mappings of the SQLite EF Core database provider
author: bricelam
ms.date: 1/26/2021
uid: core/providers/sqlite/functions
---
# Function Mappings of the SQLite EF Core Provider

This page shows which .NET members are translated into which SQL functions when using the SQLite provider.

## Binary functions

.NET                                           | SQL                                  | Added in
---------------------------------------------- | ------------------------------------ | --------
bytes.Contains(value)                          | instr(@bytes, char(@value)) > 0      | EF Core 5.0
bytes.Length                                   | length(@bytes)                       | EF Core 5.0
bytes.SequenceEqual(second)                    | @bytes = @second                     | EF Core 5.0
EF.Functions.Hex(bytes)                        | hex(@bytes)                          | EF Core 6.0
EF.Functions.Substr(bytes, startIndex)         | substr(@bytes, @startIndex)          | EF Core 6.0
EF.Functions.Substr(bytes, startIndex, length) | substr(@bytes, @startIndex, @length) | EF Core 6.0

## Conversion functions

.NET                      | SQL                           | Added in
------------------------- | ----------------------------- | --------
boolValue.ToString()      | CAST(@boolValue AS TEXT)      | EF Core 6.0
byteValue.ToString()      | CAST(@byteValue AS TEXT)      | EF Core 6.0
bytes.ToString()          | CAST(@bytes AS TEXT)          | EF Core 6.0
charValue.ToString()      | CAST(@charValue AS TEXT)      | EF Core 6.0
dateTime.ToString()       | CAST(@dateTime AS TEXT)       | EF Core 6.0
dateTimeOffset.ToString() | CAST(@dateTimeOffset AS TEXT) | EF Core 6.0
decimalValue.ToString()   | CAST(@decimalValue AS TEXT)   | EF Core 6.0
doubleValue.ToString()    | CAST(@doubleValue AS TEXT)    | EF Core 6.0
floatValue.ToString()     | CAST(@floatValue AS TEXT)     | EF Core 6.0
guid.ToString()           | CAST(@guid AS TEXT)           | EF Core 6.0
intValue.ToString()       | CAST(@intValue AS TEXT)       | EF Core 6.0
longValue.ToString()      | CAST(@longValue AS TEXT)      | EF Core 6.0
sbyteValue.ToString()     | CAST(@sbyteValue AS TEXT)     | EF Core 6.0
shortValue.ToString()     | CAST(@shortValue AS TEXT)     | EF Core 6.0
timeSpan.ToString()       | CAST(@timeSpan AS TEXT)       | EF Core 6.0
uintValue.ToString()      | CAST(@uintValue AS TEXT)      | EF Core 6.0
ushortValue.ToString()    | CAST(@ushortValue AS TEXT)    | EF Core 6.0

## Date and time functions

.NET                            | SQL                                                                      | Added in
------------------------------- | ------------------------------------------------------------------------ | --------
DateTime.Now                    | datetime('now', 'localtime')
DateTime.Today                  | datetime('now', 'localtime', 'start of day')
DateTime.UtcNow                 | datetime('now')
dateTime.AddDays(value)         | datetime(@dateTime, @value \|\| ' days')
dateTime.AddHours(value)        | datetime(@dateTime, @d \|\| ' hours')
dateTime.AddMilliseconds(value) | datetime(@dateTime, (@value / 1000.0) \|\| ' seconds')                   | EF Core 2.2
dateTime.AddMinutes(value)      | datetime(@dateTime, @value \|\| ' minutes')
dateTime.AddMonths(months)      | datetime(@dateTime, @months \|\| ' months')
dateTime.AddSeconds(value)      | datetime(@dateTime, @value \|\| ' seconds')
dateTime.AddTicks(value)        | datetime(@dateTime, (@value / 10000000.0) \|\| ' seconds')               | EF Core 2.2
dateTime.AddYears(value)        | datetime(@dateTime, @value \|\| ' years')
dateTime.Date                   | datetime(@dateTime, 'start of day')
dateTime.Day                    | strftime('%d', @dateTime)
dateTime.DayOfWeek              | strftime('%w', @dateTime)                                                | EF Core 2.2
dateTime.DayOfYear              | strftime('%j', @dateTime)
dateTime.Hour                   | strftime('%H', @dateTime)
dateTime.Millisecond            | (strftime('%f', @dateTime) * 1000) % 1000
dateTime.Minute                 | strftime('%M', @dateTime)
dateTime.Month                  | strftime('%m', @dateTime)
dateTime.Second                 | strftime('%S', @dateTime)
dateTime.Ticks                  | (julianday(@dateTime) - julianday('0001-01-01 00:00:00')) * 864000000000 | EF Core 2.2
dateTime.TimeOfDay              | time(@dateTime)                                                          | EF Core 3.0
dateTime.Year                   | strftime('%Y', @dateTime)

> [!NOTE]
> Some SQL has been simplified for illustration purposes. The actual SQL is more complex to handle a wider range of values.

## Numeric functions

.NET                  | SQL                                   | Added in
--------------------- | ------------------------------------- | --------
-decimalValue         | ef_negate(@decimalValue)              | EF Core 5.0
decimalValue - d      | ef_add(@decimalValue, ef_negate(@d))  | EF Core 5.0
decimalValue * d      | ef_multiply(@decimalValue, @d)        | EF Core 5.0
decimalValue / d      | ef_divide(@decimalValue, @d)          | EF Core 5.0
decimalValue % d      | ef_mod(@decimalValue, @d)             | EF Core 5.0
decimalValue + d      | ef_add(@decimalValue, @d)             | EF Core 5.0
decimalValue < d      | ef_compare(@decimalValue, @d) < 0     | EF Core 5.0
decimalValue <= d     | ef_compare(@decimalValue, @d) <= 0    | EF Core 5.0
decimalValue > d      | ef_compare(@decimalValue, @d) > 0     | EF Core 5.0
decimalValue >= d     | ef_compare(@decimalValue, @d) >= 0    | EF Core 5.0
doubleValue % d       | ef_mod(@doubleValue, @d)              | EF Core 5.0
EF.Functions.Random() | abs(random() / 9223372036854780000.0) | EF Core 6.0
floatValue % d        | ef_mod(@floatValue, @d)               | EF Core 5.0
Math.Abs(value)       | abs(@value)
Math.Max(val1, val2)  | max(@val1, @val2)
Math.Min(val1, val2)  | min(@val1, @val2)
Math.Round(d)         | round(@d)
Math.Round(d, digits) | round(@d, @digits)

> [!TIP]
> SQL functions prefixed with *ef* are created by EF Core.

## String functions

.NET                                                         | SQL                                                    | Added in
------------------------------------------------------------ | ------------------------------------------------------ | --------
char.ToLower(c)                                              | lower(@c)                                              | EF Core 6.0
char.ToUpper(c)                                              | upper(@c)                                              | EF Core 6.0
EF.Functions.Collate(operand, collation)                     | @operand COLLATE @collation                            | EF Core 5.0
EF.Functions.Glob(matchExpression, pattern)                  | glob(@pattern, @matchExpression)                       | EF Core 6.0
EF.Functions.Like(matchExpression, pattern)                  | @matchExpression LIKE @pattern
EF.Functions.Like(matchExpression, pattern, escapeCharacter) | @matchExpression LIKE @pattern ESCAPE @escapeCharacter
Regex.IsMatch(input, pattern)                                | regexp(@pattern, @input)                               | EF Core 6.0
string.Compare(strA, strB)                                   | CASE WHEN @strA = @strB THEN 0 ... END
string.Concat(str0, str1)                                    | @str0 \|\| @str1
string.IsNullOrEmpty(value)                                  | @value IS NULL OR @value = ''
string.IsNullOrWhiteSpace(value)                             | @value IS NULL OR trim(@value) = ''
stringValue.CompareTo(strB)                                  | CASE WHEN @stringValue = @strB THEN 0 ... END
stringValue.Contains(value)                                  | instr(@stringValue, @value) > 0
stringValue.EndsWith(value)                                  | @stringValue LIKE '%' \|\| @value
stringValue.FirstOrDefault()                                 | substr(@stringValue, 1, 1)                             | EF Core 5.0
stringValue.IndexOf(value)                                   | instr(@stringValue, @value) - 1
stringValue.LastOrDefault()                                  | substr(@stringValue, length(@stringValue), 1)          | EF Core 5.0
stringValue.Length                                           | length(@stringValue)
stringValue.Replace(oldValue, newValue)                      | replace(@stringValue, @oldValue, @newValue)
stringValue.StartsWith(value)                                | @stringValue LIKE @value \|\| '%'
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

.NET                                     | SQL                                | Added in
---------------------------------------- | ---------------------------------- | --------
collection.Contains(item)                | @item IN @collection               | EF Core 3.0
enumValue.HasFlag(flag)                  | @enumValue & @flag = @flag
nullable.GetValueOrDefault()             | coalesce(@nullable, 0)
nullable.GetValueOrDefault(defaultValue) | coalesce(@nullable, @defaultValue)

## See also

* [Spatial Function Mappings](xref:core/providers/sqlite/spatial#spatial-function-mappings)
