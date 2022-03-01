using System;
using System.Collections.Generic;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class DocumentSummaryTests
    {
        public static IEnumerable<object[]> DocSummaryTests()
        {
            var resolvers = new Func<Document, string>[]
            {
                doc => doc.Uid,
                doc => doc.Title,
                doc => doc.AuthorAlias,
            };

            var summaryResolvers = new Func<DocumentSummary, string>[]
            {
                summary => summary.Uid,
                summary => summary.Title,
                summary => summary.AuthorAlias,
            };

            for (var idx = 0; idx < resolvers.Length; idx++)
            {
                yield return new object[]
                {
                    resolvers[idx],
                    summaryResolvers[idx]
                };
            }
        }

        [Theory]
        [MemberData(nameof(DocSummaryTests))]
        public void Properties_Match_Doc(
            Func<Document, string> resolver,
            Func<DocumentSummary, string> summaryResolver)
        {
            // arrange
            var doc = new Document
            {
                Uid = nameof(DocSummaryTests),
                AuthorAlias = "system",
                Title = "Document title"
            };

            // act
            var summary = new DocumentSummary(doc);
            var expected = resolver(doc);
            var actual = summaryResolver(summary);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HashCode_Is_HashCode_Of_Uid()
        {
            // arrange
            var summary = new DocumentSummary
            {
                Uid = nameof(DocSummaryTests)
            };

            // act
            var expected = summary.Uid.GetHashCode();
            var actual = summary.GetHashCode();

            // assert
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> EqualityTests()
        {
            var uid = Guid.NewGuid().ToString();
            var sameUid = new DocumentSummary
            {
                Uid = uid
            };

            var differentUid = new DocumentSummary
            {
                Uid = Guid.NewGuid().ToString()
            };

            var anonymous = new
            {
                Uid = uid
            };

            yield return new object[]
            {
                uid,
                sameUid,
                true
            };

            yield return new object[]
            {
                uid,
                differentUid,
                false
            };

            yield return new object[]
            {
                uid,
                anonymous,
                false
            };
        }

        [Theory]
        [MemberData(nameof(EqualityTests))]
        public void Equality_Based_On_Type_And_Id(
            string uid,
            object target,
            bool areEqual)
        {
            // arrange
            var summary = new DocumentSummary
            {
                Uid = uid
            };

            // act
            var equals = summary.Equals(target);

            // assert
            Assert.Equal(areEqual, equals);
        }

        [Fact]
        public void ToString_Includes_Uid_Alias_And_Title()
        {
            // arrange
            var summary = new DocumentSummary
            {
                Uid = nameof(EqualityTests),
                Title = "Something great",
                AuthorAlias = "test"
            };

            // act
            var str = summary.ToString();

            // assert
            Assert.Contains(summary.Uid, str);
            Assert.Contains(summary.Title, str);
            Assert.Contains(summary.AuthorAlias, str);
        }
    }
}
