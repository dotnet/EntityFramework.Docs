---
title: Function Mappings - Azure Cosmos DB Provider - EF Core
description: Function Mappings of the Azure Cosmos DB EF Core Provider
author: bricelam
ms.date: 1/26/2021
uid: core/providers/cosmos/functions
---
# Function Mappings of the Azure Cosmos DB EF Core Provider

This page shows which .NET members are translated into which SQL functions when using the Azure Cosmos DB provider.

.NET                          | SQL                              | Added in
----------------------------- | -------------------------------- | --------
collection.Contains(item)     | @item IN @collection
EF.Functions.Random()         | RAND()                           | EF Core 6.0
stringValue.Contains(value)   | CONTAINS(@stringValue, @value)   | EF Core 5.0
stringValue.EndsWith(value)   | ENDSWITH(@stringValue, @value)   | EF Core 5.0
stringValue.FirstOrDefault()  | LEFT(@stringValue, 1)            | EF Core 5.0
stringValue.LastOrDefault()   | RIGHT(@stringValue, 1)           | EF Core 5.0
stringValue.StartsWith(value) | STARTSWITH(@stringValue, @value) | EF Core 5.0
