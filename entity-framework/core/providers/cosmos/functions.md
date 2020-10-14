---
title: Function Mappings - Azure Cosmos DB Provider - EF Core
description: Function Mappings of the Azure Cosmos DB EF Core Provider
author: bricelam
ms.date: 10/08/2020
uid: core/providers/cosmos/functions
---
# Function Mappings of the Azure Cosmos DB EF Core Provider

This page shows which .NET members are translated into which SQL functions when using the Azure Cosmos DB provider.

.NET                          | SQL                              | Added in
----------------------------- | -------------------------------- | --------
collection.Contains(item)     | @item IN @collection
stringValue.Contains(value)   | CONTAINS(@stringValue, @value)   | EF Core 5.0
stringValue.EndsWith(value)   | ENDSWITH(@stringValue, @value)   | EF Core 5.0
stringValue.FirstOrDefault()  | LEFT(@stringValue, 1)            | EF Core 5.0
stringValue.LastOrDefault()   | RIGHT(@stringValue, 1)           | EF Core 5.0
stringValue.StartsWith(value) | STARTSWITH(@stringValue, @value) | EF Core 5.0
