using System;
using System.Collections.Generic;
using System.IO;
using PlanetaryDocs.Domain;
using PlanetaryDocsLoader;

// path to repository (backwards works up from bin output directory)
const string DocsPath = @"..\..\..\..\..\..\..\entity-framework\";

// Azure Cosmos DB endpoint, defaults to emulator
const string EndPoint = "https://localhost:8081";

// Secret key for Azure Cosmos DB, defaults to emulator
const string AccessKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

// set to true to re-run tests without rebuilding db
var testsOnly = false;

Console.Clear();

Console.WriteLine($"Cosmos Loader for Planetary Docs executing in directory: {Environment.CurrentDirectory}");

if (!testsOnly && !Directory.Exists(DocsPath))
{
    Console.WriteLine($"Invalid path to docs: {DocsPath}");
    return;
}

List<Document> docsList = null;

if (!testsOnly)
{
    var filesToParse = FileSystemParser.FindCandidateFiles(DocsPath);
    docsList = MarkdownParser.ParseFiles(filesToParse);
}

await CosmosLoader.LoadDocumentsAsync(docsList, EndPoint, AccessKey);
