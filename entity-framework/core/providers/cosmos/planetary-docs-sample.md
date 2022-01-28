---
title: Azure Cosmos DB Provider - Planetary Docs sample - EF Core
description: An end-to-end sample to demonstrate create, read and update capabilities in the EF Core Azure Cosmos DB provider.
author: JeremyLikness
ms.author: jeliknes
ms.date: 01/27/2022
uid: core/providers/cosmos/planetary-docs-sample
---
# The Planetary Docs Sample

The Planetary Docs sample is designed to provide a comprehensive example that demonstrates how an app with create, read, and update requirements is architected with the EF Core Azure Cosmos DB provider. This Blazor Server app provides search capabilities, an interface to add and update documents, and a system to store versioned snapshots. The fact that the payloads are documents with metadata that may change over time makes it ideally suited for a document database. The database uses multiple containers with different partition keys and a mechanism to provide blazing fast, inexpensive searches for specific fields. It also handles concurrency.

## Get started

Here's how to get started in a few easy steps.

### Clone this repo

Using your preferred tools, clone the repository. The `git` commmand looks like this:

```bash
git clone https://github.com/dotnet/EntityFramework.Docs
```

### Create an Azure Cosmos DB instance

To run this demo, you will need to either run the [Azure Cosmos DB emulator](https://docs.microsoft.com/azure/cosmos-db/local-emulator)
or create an Azure Cosmos DB account. You can read
[Create an Azure Cosmos DB account](https://docs.microsoft.com/azure/cosmos-db/create-cosmosdb-resources-portal#create-an-azure-cosmos-db-account) to learn how. Be sure to check out the option
for a [free account](https://docs.microsoft.com/azure/cosmos-db/optimize-dev-test#azure-cosmos-db-free-tier)!

Choose the SQL API.

> This project is configured to use the emulator "out of the box."

### Initialize the database

Navigate to the `PlanetaryDocsLoader` project.

If you are using the emulator, make sure the emulator is running.

If you are using an Azure Cosmos DB account, update `Program.cs` with:

- The Azure Cosmos DB endpoint
- The Azure Cosmos DB key

> The endpoint is the `URI` and the key is the `Primary Key` on the **keys** pane of your Azure Cosmos DB account in the [Azure Portal](https://portal.azure.com/).

Run the application (`dotnet run` from the command line). You should see status
as it parses documents, loads them to the database and then runs tests. This step
may take several minutes.

### Configure and run the Blazor app

If you are using the emulator, the Blazor app is ready to run. If you are using an account, navigate to the `PlanetaryDocs` Blazor Server project and either update the `CosmosSettings` in the `appsettings.json` file, or create a new section in `appsettings.Development.json` and add your access key and endpoint. Run the app. You should be ready to go!

## Project Details

The following features were integrated into this project.

`PlanetaryDocsLoader` parses the docs repository and inserts the
documents into the database. It includes tests to verify the
functionality is working.

`PlanetaryDocs.Domain` hosts the domain classes, validation logic,
and signature (interface) for data access.

`PlanetaryDocs.DataAccess` contains the EF Core `DbContext`
and an implementation of the data access service.

- `DocsContext`
  - Has model-building code that shows how to map ownership
  - Uses value converters with JSON serialization to support primitives collection and nested
complex types
  - Demonstrates use of partition keys, including how to define them for the
model and how to specify them in queries
  - Provides an example of specifying the container by entity
  - Shows how to turn off the discriminator
  - Stores two entity types (aliases and tags) in the same container
  - Uses a "shadow property" to track partition keys on aliases and tags
  - Hooks into the `SavingChanges` event to automate the generation of audit snapshots
- `DocumentService`
  - Shows various strategies for C.R.U. operations
  - Programmatically synchronizes related entities
  - Demonstrates how to handle updates with concurrency to disconnected entities
  - Uses the new `IDbContextFactory<T>` implementation to manage context instances

`PlanetaryDocs` is a Blazor Server app.

- Examples of JavaScript interop in the `TitleService`, `HistoryService`, and `MultiLineEditService`.
- Uses keyboard handlers to allow keyboard-based navigation and input on the edit page
- Shows a generic autocomplete component with debounce built-in
- `HtmlPreview` uses a phantom `textarea` to render an HTML preview
- `MarkDig` is used to transform markdown into HTML
- The `MultiLineEdit` component shows a workaround using JavaScript interop for limitations with fields that have large input values
- The `Editor` component supports concurrency. If you open a document twice in separate tabs and edit in both, the second will notify that changes were made and provide the option to reset or overwrite

Your feedback is valuable! [File an issue](https://github.com/dotnet/EntityFramework.Docs/issues/new) to report defects or request changes (we also accept pull requests.)
