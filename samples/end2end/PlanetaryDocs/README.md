---
page_type: sample
description: "This repository is intended to showcase a full application that supports Create, Read, Update, and Delete operations (CRUD) using Blazor (Server), Entity Framework Core, and Azure Cosmos DB."
languages:
- csharp
products:
- ef-core
- blazor
- azure-cosmos-db
---

# Planetary Docs

Welcome to Planetary Docs! This repository is intended to showcase a full
application that supports Create, Read, Update, and Delete operations (CRUD)
using Blazor (Server), Entity Framework Core and Azure Cosmos DB.

> **Important Security Notice** This app is meant for demo purposes only. As implemented, it
is not a production-ready app. More specifically, there are no users or roles defined and
access is _not_ secured by a login. That means anyone with the URL can modify your
document database.

## Quickstart

Here's how to get started in a few easy steps.

### Clone this repo

Using your preferred tools, clone the repository. The `git` commmand looks like this:

```bash
git clone https://github.com/dotnet/EntityFramework.Docs
```

### Create an Azure Cosmos DB instance

To run this demo, you will need to either run the [Azure Cosmos DB emulator](https://learn.microsoft.com/azure/cosmos-db/local-emulator)
or create an Azure Cosmos DB account. You can read
[Create an Azure Cosmos DB account](https://learn.microsoft.com/azure/cosmos-db/create-cosmosdb-resources-portal#create-an-azure-cosmos-db-account) to learn how. Be sure to check out the option
for a [free account](https://learn.microsoft.com/azure/cosmos-db/optimize-dev-test#azure-cosmos-db-free-tier)!

Choose the SQL API.

> This project is configured to use the emulator "out of the box."

### Initialize the database

Navigate to the `PlanetaryDocsLoader` project.

If you are using the emulator, make sure the emulator is running.

If you are using an Azure Cosmos DB account, update `Program.cs` with:

- The Azure Cosmos DB endpoint
- The Azure Cosmos DB key

> The endpoint is the `URI` and the key is the `Primary Key` on the **keys** pane of your Azure
Cosmos DB account in the [Azure Portal](https://portal.azure.com/).

Run the application (`dotnet run` from the command line). You should see status
as it parses documents, loads them to the database and then runs tests. This step
may take several minutes.

### Configure and run the Blazor app

If you are using the emulator, the Blazor app is ready to run. If you are using an account,
navigate to the `PlanetaryDocs` Blazor Server project and either update the `CosmosSettings`
in the `appsettings.json` file, or create a new section in `appsettings.Development.json`
and add your access key and endpoint. Run the app. You should be ready to go!

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
  - Shows various strategies for C.R.U.D. operations
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
