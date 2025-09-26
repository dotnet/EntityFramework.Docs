# Cosmos Sample

This sample demonstrates Azure Cosmos DB features with Entity Framework Core.

## Requirements for Trigger Feature

The trigger execution feature (`HasTrigger` methods) requires:
- .NET 10.0 SDK (10.0.100-rc.1.25451.107 or later)
- EF Core 10.0.0-rc.1.25451.107 or later

The project is configured to use these versions, but the code may not compile in environments without .NET 10 SDK installed.

## Building

To build this sample, ensure you have the .NET 10 SDK installed. If not available, comment out the `HasTrigger` method calls in `TriggerSample.cs`.