using System;
using System.Collections.Generic;
using System.Text.Json;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class DocumentAuditTests
    {
        private const string Author = "jeliknes";
        private const string Markdown = "# Hi";
        private const string Html = "<h1>Hi</h1>";

        private readonly DateTime PublishDate =
            DateTime.UtcNow.AddDays(-2);

        private Document NewDoc()
        {
            var doc = new Document
            {
                AuthorAlias = Author,
                Description = $"Document by {Author}",
                Html = Html,
                Markdown = Markdown,
                PublishDate = PublishDate,
                Title = "Title of document"
            };

            doc.Tags.Add("one");
            doc.Tags.Add("two");

            return doc;
        }

        [Fact]
        public void New_Instance_With_Doc_Generates_New_Guid()
        {
            // arrange
            var doc = NewDoc();

            // act
            var docAudit = new DocumentAudit(doc);

            // assert
            Assert.NotEqual(default, docAudit.Id);
        }

        [Fact]
        public void Uid_Initializes_To_Uid_Of_Document()
        {
            // arrange
            var doc = NewDoc();

            // act
            var docAudit = new DocumentAudit(doc);

            // assert
            Assert.Equal(doc.Uid, docAudit.Uid);
        }

        [Fact]
        public void Timestamp_Defaults_To_UtcNow()
        {
            // arrange
            var doc = NewDoc();

            // act
            var docAudit = new DocumentAudit(doc);
            var now = DateTimeOffset.UtcNow;
            var diff = now - docAudit.Timestamp;
            var tolerance = TimeSpan.FromMilliseconds(50);

            // assert
            Assert.True(diff < tolerance);
        }

        [Fact]
        public void Document_Is_Json_Serialized_Snapshot()
        {
            // arrange
            var doc = NewDoc();

            // act
            var docAudit = new DocumentAudit(doc);
            var serialized = JsonSerializer.Serialize(doc);

            // assert
            Assert.Equal(serialized, docAudit.Document);
        }

        public static IEnumerable<object[]> DocumentSnapshotTests()
        {
            var resolvers = new Func<Document, string>[]
            {
                doc => doc.AuthorAlias,
                doc => doc.Description,
                doc => doc.Html,
                doc => doc.Markdown,
                doc => doc.PublishDate.Ticks.ToString(),
                doc => string.Join(", ", doc.Tags),
                doc => doc.Title,
                doc => doc.Uid
            };

            foreach (var resolver in resolvers)
            {
                yield return new object[] { resolver };
            }
        }

        [Theory]
        [MemberData(nameof(DocumentSnapshotTests))]
        public void GetDocumentSnapshot_Deserializes_Document(
            Func<Document, string> resolver)
        {
            // arrange
            var doc = NewDoc();

            // act
            var docAudit = new DocumentAudit(doc);
            var compDoc = docAudit.GetDocumentSnapshot();

            // assert
            Assert.Equal(resolver(doc), resolver(compDoc));
        }
    }
}
