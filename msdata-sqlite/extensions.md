---
title: Extensions - Microsoft.Data.Sqlite
author: bricelam
ms.date: 11/28/2019
---
# Extensions

SQLite supports loading extensions at runtime. Extensions include things like additional SQL functions, collations, virtual tables, and more.

.NET Core includes additional logic for locating native libraries in additional places like referenced NuGet packages. Unfortunately, SQLite can't leverage this logic; it calls the platform API directly to load libraries. Because of this, your app may need to modify the PATH, LD_LIBRARY_PATH, or DYLD_LIBRARY_PATH environment variables before loading SQLite extensions. There's [a sample](https://github.com/aspnet/EntityFramework.Docs/blob/master/samples/msdata-sqlite/ExtensionsSample/Program.cs) on GitHub that demonstrates finding binaries for the current runtime inside a referenced NuGet package.

To load an extension, call [LoadExtension](/dotnet/api/microsoft.data.sqlite.sqliteconnection.loadextension). Microsoft.Data.Sqlite will ensure that the extension remains loaded even if the connection is closed and reopened.

[!code-csharp[](../samples/msdata-sqlite/ExtensionsSample/Program.cs?name=snippet_LoadExtension)]

## See also

* [Run-Time Loadable Extensions <span class="docon docon-navigate-external" aria-hidden="true" />](https://www.sqlite.org/loadext.html)
