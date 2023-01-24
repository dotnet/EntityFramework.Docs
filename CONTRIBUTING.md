# Contributing to the Entity Framework documentation

The process of contributing articles and code samples to the Entity Framework documentation is explained below. Contributions can be as simple as typo corrections or as complex as new articles.

## How to make a simple correction or suggestion

Articles are stored as Markdown files in this repository. To make a simple change to the content of a Markdown file, click the **Edit** link in the upper right corner of the browser window. You might need to expand the **options** bar to see the **Edit** link. Follow the directions to create a pull request (PR). The EF team will review the PR and accept it or suggest changes.

## How to make a more complex submission

You'll need a basic understanding of [Git and GitHub.com](https://guides.github.com/activities/hello-world/).

* Open an [issue](https://github.com/dotnet/EntityFramework.Docs/issues/new) describing what you want to do, such as change an existing article or create a new one. Wait for approval from the EF team before you invest much time.
* Fork the [dotnet/EntityFramework.Docs](https://github.com/dotnet/EntityFramework.Docs/) repo and create a branch for your changes.
* Submit a pull request (PR) to main with your changes.
* Respond to PR feedback.

## Markdown syntax

Articles are written in [DocFx-flavored Markdown (DFM)](http://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html), a superset of [GitHub-flavored Markdown (GFM)](https://guides.github.com/features/mastering-markdown/). For examples of DFM syntax and metadata for UI features commonly used in the EF documentation, see [Metadata and Markdown Template](https://learn.microsoft.com/contribute/dotnet/dotnet-style-guide).

## Folder structure conventions

Images and other static content are stored in an `_static` folder within each area/folder of the site.

Code samples are stored in the `samples` root folder. They are organized into a folder structure that mimics the documentation structure (found under the `entity-framework` root folder).

## Code snippets

Articles frequently contain code snippets to illustrate points. DFM lets you copy code into the Markdown file or refer to a separate code file. Whenever possible, use separate code files to minimize the chance of errors in the code. The code files should be stored in the repo using the folder structure described above for sample projects.

Here are some examples of [DFM code snippet syntax](http://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html#code-snippet).

To render an entire code file as a snippet:

```none
[!code-csharp[Main](../../../samples/core/saving/Program.cs)]
```

To render a portion of a file as a snippet by using line numbers:

```none
[!code-csharp[Main](../../../samples/core/saving/Program.cs?range=1-10]
```

For C# snippets, you can reference a [C# region](https://msdn.microsoft.com/library/9a1ybwek.aspx). Use regions rather than line numbers. Line numbers in a code file tend to change and get out of sync with line number references in Markdown. C# regions can be nested. If you reference the outer region, the inner `#region` and `#endregion` directives are not rendered in a snippet.

To render a C# region named "snippet_Example":

```none
[!code-csharp[Main](../../../samples/core/saving/Program.cs?name=snippet_Example)]
```

To highlight selected lines in a rendered snippet (usually renders as yellow background color):

```none
[!code-csharp[Main](../../../samples/core/saving/Program.cs?name=snippet_Example&highlight=1-3,10,20-25)]
```

## Test your changes with DocFX

Test your changes with the [DocFX command-line tool](https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html#2-use-docfx-as-a-command-line-tool), which creates a locally hosted version of the site. DocFX doesn't render style and site extensions created for Microsoft Learn.

DocFX requires the .NET Framework on Windows, or Mono for Linux or macOS.

### Windows instructions

* Download and unzip *docfx.zip* from [DocFX releases](https://github.com/dotnet/docfx/releases).
* Add DocFX to your PATH.
* In a command-line window, navigate to the cloned repository (which contains the *docfx.json* file) and run the following command:

   ```console
   docfx -t default --serve
   ```

* In a browser, navigate to `http://localhost:8080`.

### Mono instructions

* Install Mono via Homebrew - `brew install mono`.
* Download the [latest version of DocFX](https://github.com/dotnet/docfx/releases/tag/v2.7.2).
* Extract to `\bin\docfx`.
* Create an alias for **docfx**:

  ```console
  function docfx {
    mono $HOME/bin/docfx/docfx.exe
  }

  function docfx-serve {
    mono $HOME/bin/docfx/docfx.exe serve _site
  }
  ```

* Run **docfx** in the cloned repository to build the site, and **docfx-serve** to view the site at `http://localhost:8080`.

## Voice and tone

Our goal is to write documentation that is easily understandable by the widest possible audience. To that end we have established guidelines for writing style that we ask our contributors to follow. For more information, see [Voice and tone guidelines](https://learn.microsoft.com/contribute/dotnet/dotnet-voice-tone).
