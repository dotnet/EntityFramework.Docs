using System;
using System.Collections.Generic;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class AuthorTests
    {
        [Fact]
        public void New_Instance_Initializes_Documents()
        {
            // arange and act
            var author = new Author();

            // assert
            Assert.NotNull(author.Documents);
        }

        [Fact]
        public void HashCode_Of_Author_Is_HashCode_Of_Alias()
        {
            // arrange
            const string alias = nameof(AuthorTests);
            var author = new Author { Alias = alias };

            // act
            var expected = alias.GetHashCode();
            var actual = author.GetHashCode();

            // assert
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> AuthorEqualityTests()
        {
            yield return new object[]
            {
                new Author { Alias = nameof(AuthorTests) }, true
            };

            yield return new object[]
            {
                new Author { Alias = nameof(AuthorEqualityTests) }, false
            };

            yield return new object[]
            {
                new object(), false
            };

            yield break;
        }

        [Theory]
        [MemberData(nameof(AuthorEqualityTests))]
        public void Equality_Compares_Type_And_Alias(object compare, bool equal)
        {
            // arrange
            var author = new Author { Alias = nameof(AuthorTests) };

            // act
            var areEqual = author.Equals(compare);

            // assert
            Assert.Equal(equal, areEqual);
        }

        [Theory]
        [InlineData("jeremy", 1)]
        [InlineData("jeremy", 2)]
        [InlineData("randomperson", 3)]
        public void Author_ToString_Includes_Alias_And_Docs_Count(
            string alias,
            int expectedDocs)
        {
            // arrange
            var author = new Author { Alias = alias };
            for (var idx = 0; idx < expectedDocs; idx++)
            {
                var summary = new DocumentSummary
                {
                    AuthorAlias = alias,
                    Title = Guid.NewGuid().ToString(),
                };
                summary.Uid = summary.Title.Split('-')[^1];
                author.Documents.Add(summary);
            }
        }
    }
}
