using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig;
using PlanetaryDocs.Domain;

namespace PlanetaryDocsLoader
{
    /// <summary>
    /// Manages transformation of markdown to HTML.
    /// </summary>
    public static class MarkdownParser
    {
        private static readonly Regex NotAlphanumeric = new ("[^a-zA-Z0-9]");

        /// <summary>
        /// Parses a list of files to a list of <see cref="Document"/> instances.
        /// </summary>
        /// <param name="filesToParse">The file list.</param>
        /// <returns>The parsed <see cref="Document"/> list.</returns>
        public static List<Document> ParseFiles(
            IEnumerable<string> filesToParse)
        {
            var docsList = new List<Document>();

            Console.WriteLine("Loading stop words...");
            var stopWords = File.ReadAllLines("stopwords.txt");

            Console.WriteLine($"{stopWords.Length} stop words loaded.");

            // pass 1: compute word frequencies to generate tags
            var words = new Dictionary<string, int>();
            var wordsPerDocument = new Dictionary<string, IDictionary<string, int>>();

            var total = filesToParse.Count();
            var progress = new ProgressBar(total);
            var blank = new string(' ', Console.BufferWidth - 1);
            var (left, top) = Console.GetCursorPosition();

            foreach (var file in filesToParse)
            {
                Console.SetCursorPosition(left, top);
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine(blank);
                }

                Console.SetCursorPosition(left, top);
                Console.WriteLine($"{progress.Advance()}");
                Console.WriteLine("===");
                var fileName = file.Split(Path.DirectorySeparatorChar)[^1];
                Console.WriteLine($"DOC:\t{fileName}");
                var lines = File.ReadAllLines(file);
                Console.WriteLine($"LINES:\t{lines.Length}");
                var doc = new Document();
                bool metaStart = false,
                    metaEnd = false,
                    titleFound = false,
                    aliasFound = false,
                    descriptionFound = false,
                    dateFound = false,
                    uidFound = false;

                var markdown = new List<string>();

                for (var idx = 0; idx < lines.Length; idx++)
                {
                    var line = lines[idx];

                    if (!metaStart)
                    {
                        if (line.StartsWith("---"))
                        {
                            metaStart = true;
                        }

                        continue;
                    }

                    if (!metaEnd)
                    {
                        if (line.StartsWith("---"))
                        {
                            metaEnd = true;
                            continue;
                        }
                        else
                        {
                            var metadata = line.Split(":");
                            var key = metadata[0].Trim().ToLowerInvariant();
                            switch (key)
                            {
                                case "title":
                                    titleFound = true;
                                    doc.Title = metadata[1].Trim();
                                    Console.WriteLine($"TITLE:\t{doc.Title}");
                                    break;

                                case "uid":
                                    uidFound = true;
                                    doc.Uid = metadata[1].Trim().Replace('/', '_');
                                    Console.WriteLine($"UID:\t{doc.Uid}");
                                    break;

                                case "description":
                                    descriptionFound = true;
                                    doc.Description = metadata[1].Trim();
                                    break;

                                case "ms.author":
                                case "author":
                                    aliasFound = true;
                                    doc.AuthorAlias = metadata[1].Trim();
                                    Console.WriteLine($"AUTHOR:\t{doc.AuthorAlias}");
                                    break;

                                case "ms.date":
                                    dateFound = true;
                                    doc.PublishDate = DateTime.ParseExact(
                                        metadata[1].Trim(),
                                        "M/d/yyyy",
                                        CultureInfo.InvariantCulture);
                                    Console.WriteLine($"PUB DATE:\t{doc.PublishDate}");
                                    break;

                                case "default":
                                    continue;
                            }

                            continue;
                        }
                    }

                    markdown.Add(line);
                    ParseWords(file, line, words, wordsPerDocument, stopWords);
                }

                var valid = titleFound && aliasFound && descriptionFound && dateFound
                    && uidFound;

                if (valid)
                {
                    Console.WriteLine("VALID");
                    doc.Markdown = string.Join(Environment.NewLine, markdown);
                    doc.Html = Markdown.ToHtml(doc.Markdown);
                    docsList.Add(doc);

                    // change from file to uid reference
                    if (wordsPerDocument.ContainsKey(file))
                    {
                        wordsPerDocument.Add(doc.Uid, wordsPerDocument[file]);
                        wordsPerDocument.Remove(file);
                    }
                }
                else
                {
                    Console.WriteLine("INVALID");
                    continue;
                }
            }

            Console.WriteLine("Generating tags...");
            GenerateTags(docsList, words, wordsPerDocument);

            Console.WriteLine($"Processed {docsList.Count} documents.");
            return docsList;
        }

        private static void GenerateTags(
            List<Document> docsList,
            Dictionary<string, int> words,
            Dictionary<string, IDictionary<string, int>> wordsPerDocument)
        {
            foreach (Document doc in docsList)
            {
                doc.Tags = wordsPerDocument[doc.Uid].Join(
                    words,
                    wpd => wpd.Key,
                    w => w.Key,
                    (w, wpd) => new
                    {
                        word = w.Key,
                        idf = (double)wpd.Value / w.Value,
                    })
                    .OrderByDescending(words => words.idf)
                    .Take(10)
                    .Select(w => w.word)
                    .ToList();
            }
        }

        private static void ParseWords(
            string file,
            string line,
            IDictionary<string, int> words,
            IDictionary<string, IDictionary<string, int>> wordsPerDocument,
            IEnumerable<string> stopWords)
        {
            var text = NotAlphanumeric.Replace(line, " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var hit in text)
            {
                var newWord = hit.ToLowerInvariant().Trim();
                if (newWord.Length < 4 || stopWords.Contains(newWord))
                {
                    continue;
                }

                if (words.ContainsKey(newWord))
                {
                    words[newWord]++;
                }
                else
                {
                    words.Add(newWord, 1);
                }

                if (!wordsPerDocument.ContainsKey(file))
                {
                    wordsPerDocument.Add(file, new Dictionary<string, int>());
                }

                if (wordsPerDocument[file].ContainsKey(newWord))
                {
                    words[newWord]++;
                }
                else
                {
                    wordsPerDocument[file].Add(newWord, 1);
                }
            }
        }
    }
}
