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

Using your preferred tools, clone the repository. The `git` command looks like this:

```bash
git clone https://github.com/dotnet/EntityFramework.Docs
```

### Create an Azure Cosmos DB instance

To run this demo, you will need to either run the [Azure Cosmos DB emulator](/azure/cosmos-db/local-emulator)
or create an Azure Cosmos DB account. You can read
[Create an Azure Cosmos DB account](/azure/cosmos-db/create-cosmosdb-resources-portal#create-an-azure-cosmos-db-account) to learn how. Be sure to check out the option
for a [free account](/azure/cosmos-db/optimize-dev-test#azure-cosmos-db-free-tier)!

Choose the **SQL API**.

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

## Introducing Planetary Docs

You may (or may not) know that Microsoft's [official documentation](/) runs entirely on open source. It uses markdown with some metadata enhancements to build the interactive documentation that .NET developers use daily. The hypothetical scenario for Planetary Docs is to provide a web-based tool for authoring the docs. It allows setting up the title, description, the alias of the author, assigning tags, editing markdown and previewing the HTML output.

It's planetary because Azure Cosmos DB is "planetary scale". The app provides the ability to search documents. Documents are stored under aliases and tags for fast lookup, but full text search is available as well. The app automatically audits the documents (it takes snapshots of the document anytime it is edited and provides a view of the history).

> [!NOTE]
> Delete and restore aren't implemented.

Here's a look at the document for `Document`:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.Domain/Document.cs":::

For faster lookups, the `DocumentSummary` class contains some basic information about the document.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.Domain/DocumentSummary.cs":::

This is used by both `Author` and `Tag`. They look pretty similar. Here's the `Tag` code:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.Domain/Tag.cs":::

The `ETag` property is implemented on the model so it can be sent around the app and maintain the value (as opposed to using a [shadow property](xref:core/modeling/shadow-properties) ). The `ETag` is used [for concurrency](/dotnet/api/microsoft.azure.documents.resource.etag) in Azure Cosmos DB. Concurrency support is implemented in the sample app. To test it, try opening the same document in two tabs, then update one and save it, and finally update the other and save it.

People often struggle with [disconnected entities](xref:core/saving/disconnected-entities) in EF Core. The pattern is used in this app to provide an example. It's not necessary in Blazor Server, but makes it easier to scale the app. The alternative approach is to track the state of the entity with EF Core's [change tracker](xref:core/change-tracking/index). The change tracker would enable you to drop the `ETag` property and use a shadow property instead.

Finally, there is the `DocumentAudit` document.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.Domain/DocumentAudit.cs":::

Ideally, the `Document` snapshot would be a proper property instead of a string. This is one of the [EF Core Azure Cosmos DB provider limitations](xref:core/providers/cosmos/limitations) EF Core currently has. There is not a way for `Document` to do double-duty as both a standalone entity and an "owned" entity. If you want the user to be able to search on properties in the historical document, you could either add those properties to the `DocumentAudit` class to be automatically indexed, or make a `DocumentSnapshot` class that shares the same properties but is configured as "owned" by the `DocumentAudit` parent.

### Azure Cosmos DB setup

The strategy for the data store is to use three containers.

One container named `Documents` is dedicated exclusively to documents. They are partitioned by `id`. That's one partition per document. [Here's the rationale](/azure/cosmos-db/partitioning-overview#using-item-id-as-the-partition-key).

The audits are stored in a container named `Audits`. The partition key is the document id, so all histories are stored in the same partition. This allows for fast, single-partition queries over historical data.

Finally, there is some metadata that is stored in `Meta`. The partition key is the meta data type, either `Author` or `Tag`. The metadata contains summaries of the related documents. If the user wants to search for documents with tag `x` the app doesn't have to scan all documents. Instead, it reads the document for tag `x` that contains a collection of the related documents it is tagged in. This approach is fast for read but does require some additional work on writes and updates that will be covered later.

### Entity Framework Core

The `DbContext` for Planetary Docs is named `DocsContext` in the `PlanetaryDocs.DataAccess` project. It has a constructor that takes a `DbContextOptions<DocsContext>` parameter and passes it to the base class to enable run-time configuration.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocsContext.cs" range="31-33":::

The `DbSet<>` generic type is used to specify the classes that should be persisted.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocsContext.cs" range="35-53":::

A few helper methods on the `DbContext` class make it easier to search for and assign metadata. Both metadata items use a string-based key and specify the type as the partition key. This enables a generic strategy to find records:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocsContext.cs" range="89-106":::

`FindAsync` (the existing method on the base `DbContext` that is shipped as part of EF Core) doesn't require closing the type to specify the key. It takes it as an `object` parameter and applies it based on the internal representation of the model.

The entities are configured in the `OnModelCreating` overload. Here's the first configuration for `DocumentAudit`.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocsContext.cs" range="157-161":::

This configuration informs EF Core that...

- There will only be one type stored in the table, so there is no need for a discriminator to distinguish types.
- The documents should be stored in a container named `Audits`.
- The partition key is the document id.
- The access key is the unique identifier of the audit combined with the partition key (the unique identifier of the document).

Next, the `Document` is configured:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocsContext.cs" range="163-171":::

Here we specify a few more details, such as how the `ETag` property should be mapped:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocsContext.cs" range="173-183":::

The partition key is configured as a shadow property. Unlike the `ETag` property, the partition key is fixed and therefore doesn't have to live on the model. Read this to learn more about models in EF Core: [Creating and configuring a model](xref:core/modeling/index).

The `SaveChanges` event is used that to automatically insert a document snapshot any time a document is inserted or updated. Every time changes are saved and the event is fired, the `ChangeTracker` is queried to find an `Document` entities that were added or updated. An audit entry is inserted for each one.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocsContext.cs" range="204-219":::

Doing it this way will ensure audits are generated even if you build other apps that share the same `DbContext`.

### The Data Service

A common question is whether developers should use the repository pattern with EF Core, and the answer is, "It depends." To the extent that the `DbContext` is testable and can be interfaced for mocking, there are many cases when using it directly is perfectly fine. Whether or not you specifically use the repository pattern, adding a data access layer often makes sense when there are database-related tasks to do outside of the EF Core functionality. In this example, there is database-related logic that makes more sense to isolate rather than bloating the `DbContext`, so it is implemented in `DocumentService`.

The service constructor is passed a `DbContext` factory. This is provided by EF Core to easily create new contexts using your preferred configuration. The app uses a "context per operation" rather than using long-lived contexts and change tracking. Here's the configuration to grab the settings and tell the factory to make contexts that connect to Azure Cosmos DB. The factory is then automatically injected into the service.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs/Startup.cs" range="39-54":::

Using this pattern demonstrates disconnected entities and also builds some resiliency against the case when your [Blazor SignalR circuit](/aspnet/core/blazor/fundamentals/signalr) may break.

#### Load a document

The document load is intended to get a snapshot that isn't tracked for changes because those will be sent in a separate operation. The main requirement is to set the partition key.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="293-298":::

#### Query documents

The document query allows the user to search on text anywhere in the document and to further filter by author and/or tag. The pseudo code looks like this:

- If there is a tag, load the tag and use the document summary list as the result set
  - If there is also an author, load the author and filter the results to the intersection of results between tag and author
    - If there is text, load the documents that match the text then filter the results to the author and tag intersection
  - If there is also text, load the documents that match the text then filter the results to the tag results
- Else if there is an author, load the author and filter the results to the document summary list as the result set
  - If there is text, load the documents that match the text then filter the results to the author results
- Else load the documents that match the text

Performance-wise, a tag and/or author-based search only requires one or two documents to be loaded. A text search always loads matching documents and then further filters the list based on the existing documents, so it is significantly slower (but still fast).

Here's the implementation. Note the `HashSet` works due to the `Equals` and `GetHashCode` overrides:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="65-142":::

### Create a document

Ordinarily, creating a document with EF Core would be as easy as:

```csharp
context.Add(document);
await context.SaveChangesAsync();
```

For `PlanetaryDocs` however, the document can have associated tags and an author. These have summaries that must be updated explicitly because there are no formal relationships.

> [!NOTE]
> This example uses code to keep documents in sync. If the database is used by multiple applications and services, it may make more sense to implement the logic at the database level and use [triggers and stored procedures](/azure/cosmos-db/how-to-write-stored-procedures-triggers-udfs) instead.

A generic method handles keeping the documents in sync. The pseudo code is the same whether it is for an author or a tag:

- If the document was inserted or updated
  - A new document will result in "author changed" and "tags added"
  - If the author was changed or a tag removed
    - Load the metadata document for the old author or removed tag
    - Remove the document from the summary list
  - If the author was changed
    - Load the metadata document for the new author
    - Add the document to the summary list
      - Load all tags for the model
      - Update the author in the summary list for each tag
  - If tags were added
    - If tag exists
      - Load the metadata document for the tag
      - Add the document to the summary list
    - Else
      - Create a new tag with the document in the summary list
  - If the document was updated and the title changed
    - Load the metadata for the existing author and/or tags
    - Update the title in the summary list

This algorithm is an example of how EF Core shines. All of these manipulations can happen in a single pass. If a tag is referenced multiple times, it is only ever loaded once. The final call to save changes will commit all changes including inserts.

Here's the code for handling changes to tags that is called as part of the insert process:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="322-401":::

The algorithm as implemented works for inserts, updates, and deletes.

#### Update a document

Now that the metadata sync has been implemented, the update code is simply:

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="189-198":::

Concurrency works in this scenario because we persist the loaded version of the entity in the `ETag` property.

#### Delete a document

The delete code uses a simplified algorithm to remove existing tag and author references.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="247-272":::

#### Search metadata (tags or authors)

Finding tags or authors that match a text string is a straightforward query. The key is to improve performance and reduce the cost of the query by making it a single partition query.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="149-161":::

The `ComputePartitionKey` method returns the simple type name as the partition. The authors list is not long, so the code pulls the aliases first, then applies an in-memory filter for the _contains_ logic.

### Deal with document audits

The last set of APIs deal with the automatically generated audits. This method loads document audits then projects them onto a summary. The projection is not done in the query because it requires deserializing the snapshot. Instead, the list of audits is obtained, then snapshots are deserialized to pull out the relevant data to display such as title and author.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="205-215":::

The `ToListAsync` materializes the query results and everything after is manipulated in memory.

The app also lets you review an audit record using the same viewer control that live documents use. A method loads the audit, materializes the snapshot and returns a `Document` entity for the view to use.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="223-240":::

Finally, although you can delete a record, the audits remain. The web app doesn't implement this yet, but it is implemented in the data service. The steps are simply deserialize the requested version and insert it.

:::code language="csharp" source="../../../../samples/end2end/PlanetaryDocs/PlanetaryDocs.DataAccess/DocumentService.cs" range="280-285":::

## Conclusion

The goal behind the sample is to provide some guidance for using the EF Core Azure Cosmos DB provider and to demonstrate the areas it shines. Please [file an issue](https://github.com/dotnet/EntityFramework.Docs/issues/new) with feedback or suggestions. We accept pull requests, so if you want to implement missing functionality or see an area of improvement, let us know!
