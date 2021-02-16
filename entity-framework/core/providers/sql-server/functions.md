---
title: Function Mappings - Microsoft SQL Server Database Provider - EF Core
description: Function Mappings of the Microsoft SQL Server database provider
author: bricelam
ms.date: 1/26/2021
uid: core/providers/sql-server/functions
---
# Function Mappings of the Microsoft SQL Server Provider

This page shows which .NET members are translated into which SQL functions when using the SQL Server provider.

## Binary functions

.NET                         | SQL                           | Added in
---------------------------- | ----------------------------- | --------
bytes.Contains(value)        | CHARINDEX(@value, @bytes) > 0 | EF Core 5.0
bytes.First()                | SUBSTRING(@bytes, 1, 1)       | EF Core 6.0
bytes.Length                 | DATALENGTH(@bytes)            | EF Core 5.0
bytes.SequenceEqual(second)  | @bytes = @second              | EF Core 5.0
bytes[i]                     | SUBSTRING(@bytes, @i + 1, 1)  | EF Core 6.0
EF.Functions.DataLength(arg) | DATALENGTH(@arg)              | EF Core 5.0

## Conversion functions

.NET                      | SQL                                    | Added in
------------------------- | -------------------------------------- | --------
bytes.ToString()          | CONVERT(varchar(100), @bytes)
byteValue.ToString()      | CONVERT(varchar(3), @byteValue)
charValue.ToString()      | CONVERT(varchar(1), @charValue)
Convert.ToBoolean(value)  | CONVERT(bit, @value)                   | EF Core 5.0
Convert.ToByte(value)     | CONVERT(tinyint, @value)
Convert.ToDecimal(value)  | CONVERT(decimal(18, 2), @value)
Convert.ToDouble(value)   | CONVERT(float, @value)
Convert.ToInt16(value)    | CONVERT(smallint, @value)
Convert.ToInt32(value)    | CONVERT(int, @value)
Convert.ToInt64(value)    | CONVERT(bigint, @value)
Convert.ToString(value)   | CONVERT(nvarchar(max), @value)
dateTime.ToString()       | CONVERT(varchar(100), @dateTime)
dateTimeOffset.ToString() | CONVERT(varchar(100), @dateTimeOffset)
decimalValue.ToString()   | CONVERT(varchar(100), @decimalValue)
doubleValue.ToString()    | CONVERT(varchar(100), @doubleValue)
floatValue.ToString()     | CONVERT(varchar(100), @floatValue)
guid.ToString()           | CONVERT(varchar(36), @guid)
intValue.ToString()       | CONVERT(varchar(11), @intValue)
longValue.ToString()      | CONVERT(varchar(20), @longValue)
sbyteValue.ToString()     | CONVERT(varchar(4), @sbyteValue)
shortValue.ToString()     | CONVERT(varchar(6), @shortValue)
timeSpan.ToString()       | CONVERT(varchar(100), @timeSpan)
uintValue.ToString()      | CONVERT(varchar(10), @uintValue)
ulongValue.ToString()     | CONVERT(varchar(19), @ulongValue)
ushortValue.ToString()    | CONVERT(varchar(5), @ushortValue)

## Date and time functions

.NET                                                        | SQL                                                  | Added in
----------------------------------------------------------- | ---------------------------------------------------- | --------
DateTime.Now                                                | GETDATE()
DateTime.Today                                              | CONVERT(date, GETDATE())
DateTime.UtcNow                                             | GETUTCDATE()
dateTime.AddDays(value)                                     | DATEADD(day, @value, @dateTime)
dateTime.AddHours(value)                                    | DATEADD(hour, @value, @dateTime)
dateTime.AddMilliseconds(value)                             | DATEADD(millisecond, @value, @dateTime)
dateTime.AddMinutes(value)                                  | DATEADD(minute, @value, @dateTime)
dateTime.AddMonths(months)                                  | DATEADD(month, @months, @dateTime)
dateTime.AddSeconds(value)                                  | DATEADD(second, @value, @dateTime)
dateTime.AddYears(value)                                    | DATEADD(year, @value, @dateTime)
dateTime.Date                                               | CONVERT(date, @dateTime)
dateTime.Day                                                | DATEPART(day, @dateTime)
dateTime.DayOfYear                                          | DATEPART(dayofyear, @dateTime)
dateTime.Hour                                               | DATEPART(hour, @dateTime)
dateTime.Millisecond                                        | DATEPART(millisecond, @dateTime)
dateTime.Minute                                             | DATEPART(minute, @dateTime)
dateTime.Month                                              | DATEPART(month, @dateTime)
dateTime.Second                                             | DATEPART(second, @dateTime)
dateTime.TimeOfDay                                          | CONVERT(time, @dateTime)                              | EF Core 2.2
dateTime.Year                                               | DATEPART(year, @dateTime)
DateTimeOffset.Now                                          | SYSDATETIMEOFFSET()
DateTimeOffset.UtcNow                                       | SYSUTCDATETIME()
dateTimeOffset.AddDays(days)                                | DATEADD(day, @days, @dateTimeOffset)
dateTimeOffset.AddHours(hours)                              | DATEADD(hour, @hours, @dateTimeOffset)
dateTimeOffset.AddMilliseconds(milliseconds)                | DATEADD(millisecond, @milliseconds, @dateTimeOffset)
dateTimeOffset.AddMinutes(minutes)                          | DATEADD(minute, @minutes, @dateTimeOffset)
dateTimeOffset.AddMonths(months)                            | DATEADD(month, @months, @dateTimeOffset)
dateTimeOffset.AddSeconds(seconds)                          | DATEADD(second, @seconds, @dateTimeOffset)
dateTimeOffset.AddYears(years)                              | DATEADD(year, @years, @dateTimeOffset)
dateTimeOffset.Date                                         | CONVERT(date, @dateTimeOffset)
dateTimeOffset.Day                                          | DATEPART(day, @dateTimeOffset)
dateTimeOffset.DayOfYear                                    | DATEPART(dayofyear, @dateTimeOffset)
dateTimeOffset.Hour                                         | DATEPART(hour, @dateTimeOffset)
dateTimeOffset.Millisecond                                  | DATEPART(millisecond, @dateTimeOffset)
dateTimeOffset.Minute                                       | DATEPART(minute, @dateTimeOffset)
dateTimeOffset.Month                                        | DATEPART(month, @dateTimeOffset)
dateTimeOffset.Second                                       | DATEPART(second, @dateTimeOffset)
dateTimeOffset.TimeOfDay                                    | CONVERT(time, @dateTimeOffset)                        | EF Core 2.2
dateTimeOffset.Year                                         | DATEPART(year, @dateTimeOffset)
EF.Functions.DateDiffDay(start, end)                        | DATEDIFF(day, @start, @end)
EF.Functions.DateDiffHour(start, end)                       | DATEDIFF(hour, @start, @end)
EF.Functions.DateDiffMicrosecond(start, end)                | DATEDIFF(microsecond, @start, @end)
EF.Functions.DateDiffMillisecond(start, end)                | DATEDIFF(millisecond, @start, @end)
EF.Functions.DateDiffMinute(start, end)                     | DATEDIFF(minute, @start, @d2)
EF.Functions.DateDiffMonth(start, end)                      | DATEDIFF(month, @start, @end)
EF.Functions.DateDiffNanosecond(start, end)                 | DATEDIFF(nanosecond, @start, @end)
EF.Functions.DateDiffSecond(start, end)                     | DATEDIFF(second, @start, @end)
EF.Functions.DateDiffWeek(start, end)                       | DATEDIFF(week, @start, @end)                         | EF Core 5.0
EF.Functions.DateDiffYear(start, end)                       | DATEDIFF(year, @start, @end)
EF.Functions.DateFromParts(year, month, day)                | DATEFROMPARTS(@year, @month, @day)                   | EF Core 5.0
EF.Functions.DateTime2FromParts(year, month, day, ...)      | DATETIME2FROMPARTS(@year, @month, @day, ...)         | EF Core 5.0
EF.Functions.DateTimeFromParts(year, month, day, ...)       | DATETIMEFROMPARTS(@year, @month, @day, ...)          | EF Core 5.0
EF.Functions.DateTimeOffsetFromParts(year, month, day, ...) | DATETIMEOFFSETFROMPARTS(@year, @month, @day, ...)    | EF Core 5.0
EF.Functions.IsDate(expression)                             | ISDATE(@expression)                                  | EF Core 3.0
EF.Functions.SmallDateTimeFromParts(year, month, day, ...)  | SMALLDATETIMEFROMPARTS(@year, @month, @day, ...)     | EF Core 5.0
EF.Functions.TimeFromParts(hour, minute, second, ...)       | TIMEFROMPARTS(@hour, @minute, @second, ...)          | EF Core 5.0
timeSpan.Hours                                              | DATEPART(hour, @timeSpan)                            | EF Core 5.0
timeSpan.Milliseconds                                       | DATEPART(millisecond, @timeSpan)                     | EF Core 5.0
timeSpan.Minutes                                            | DATEPART(minute, @timeSpan)                          | EF Core 5.0
timeSpan.Seconds                                            | DATEPART(second, @timeSpan)                          | EF Core 5.0

## Numeric functions

.NET                    | SQL                  | Added in
----------------------- | -------------------- | --------
EF.Functions.Random()   | RAND()               | EF Core 6.0
Math.Abs(value)         | ABS(@value)
Math.Acos(d)            | ACOS(@d)
Math.Asin(d)            | ASIN(@d)
Math.Atan(d)            | ATAN(@d)
Math.Atan2(y, x)        | ATN2(@y, @x)
Math.Ceiling(d)         | CEILING(@d)
Math.Cos(d)             | COS(@d)
Math.Exp(d)             | EXP(@d)
Math.Floor(d)           | FLOOR(@d)
Math.Log(d)             | LOG(@d)
Math.Log(a, newBase)    | LOG(@a, @newBase)
Math.Log10(d)           | LOG10(@d)
Math.Pow(x, y)          | POWER(@x, @y)
Math.Round(d)           | ROUND(@d, 0)
Math.Round(d, decimals) | ROUND(@d, @decimals)
Math.Sign(value)        | SIGN(@value)
Math.Sin(a)             | SIN(@a)
Math.Sqrt(d)            | SQRT(@d)
Math.Tan(a)             | TAN(@a)
Math.Truncate(d)        | ROUND(@d, 0, 1)

## String functions

.NET                                                                    | SQL                                                                    | Added in
----------------------------------------------------------------------- | ---------------------------------------------------------------------- | --------
EF.Functions.Collate(operand, collation)                                | @operand COLLATE @collation                                            | EF Core 5.0
EF.Functions.Contains(propertyReference, searchCondition)               | CONTAINS(@propertyReference, @searchCondition)                         | EF Core 2.2
EF.Functions.Contains(propertyReference, searchCondition, languageTerm) | CONTAINS(@propertyReference, @searchCondition, LANGUAGE @languageTerm) | EF Core 2.2
EF.Functions.FreeText(propertyReference, freeText)                      | FREETEXT(@propertyReference, @freeText)
EF.Functions.FreeText(propertyReference, freeText, languageTerm)        | FREETEXT(@propertyReference, @freeText, LANGUAGE @languageTerm)
EF.Functions.IsNumeric(expression)                                      | ISNUMERIC(@expression)                                                 | EF Core 6.0
EF.Functions.Like(matchExpression, pattern)                             | @matchExpression LIKE @pattern
EF.Functions.Like(matchExpression, pattern, escapeCharacter)            | @matchExpression LIKE @pattern ESCAPE @escapeCharacter
string.Compare(strA, strB)                                              | CASE WHEN @strA = @strB THEN 0 ... END
string.Concat(str0, str1)                                               | @str0 + @str1
string.IsNullOrEmpty(value)                                             | @value IS NULL OR @value LIKE N''
string.IsNullOrWhiteSpace(value)                                        | @value IS NULL OR LTRIM(RTRIM(@value)) = N''
stringValue.CompareTo(strB)                                             | CASE WHEN @stringValue = @strB THEN 0 ... END
stringValue.Contains(value)                                             | @stringValue LIKE N'%' + @value + N'%'
stringValue.EndsWith(value)                                             | @stringValue LIKE N'%' + @value
stringValue.FirstOrDefault()                                            | SUBSTRING(@stringValue, 1, 1)                                          | EF Core 5.0
stringValue.IndexOf(value)                                              | CHARINDEX(@value, @stringValue) - 1
stringValue.LastOrDefault()                                             | SUBSTRING(@stringValue, LEN(@stringValue), 1)                          | EF Core 5.0
stringValue.Length                                                      | LEN(@stringValue)
stringValue.Replace(@oldValue, @newValue)                               | REPLACE(@stringValue, @oldValue, @newValue)
stringValue.StartsWith(value)                                           | @stringValue LIKE @value + N'%'
stringValue.Substring(startIndex, length)                               | SUBSTRING(@stringValue, @startIndex + 1, @length)
stringValue.ToLower()                                                   | LOWER(@stringValue)
stringValue.ToUpper()                                                   | UPPER(@stringValue)
stringValue.Trim()                                                      | LTRIM(RTRIM(@stringValue))
stringValue.TrimEnd()                                                   | RTRIM(@stringValue)
stringValue.TrimStart()                                                 | LTRIM(@stringValue)

## Miscellaneous functions

.NET                                     | SQL                                | Added in
---------------------------------------- | ---------------------------------- | --------
collection.Contains(item)                | @item IN @collection               | EF Core 3.0
enumValue.HasFlag(flag)                  | @enumValue & @flag = @flag
Guid.NewGuid()                           | NEWID()
nullable.GetValueOrDefault()             | COALESCE(@nullable, 0)
nullable.GetValueOrDefault(defaultValue) | COALESCE(@nullable, @defaultValue)

> [!NOTE]
> Some SQL has been simplified for illustration purposes. The actual SQL is more complex to handle a wider range of values.

## See also

* [Spatial Function Mappings](xref:core/providers/sql-server/spatial#spatial-function-mappings)
