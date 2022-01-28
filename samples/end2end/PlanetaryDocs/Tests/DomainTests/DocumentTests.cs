using System;
using System.Collections.Generic;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class DocumentTests
    {
        [Fact]
        public void Tags_Are_Initialized()
        {
            // arrange and act
            var doc = new Document();

            // assert
            Assert.NotNull(doc.Tags);
        }

        [Fact]
        public void HashCode_Is_Uid_HashCode()
        {
            // arrange
            const string uid = nameof(DocumentTests);
            var doc = new Document { Uid = uid };
            var expected = uid.GetHashCode();

            // act
            var actual = doc.GetHashCode();

            // assert
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> DocEqualityTests()
        {
            var uid = Guid.NewGuid().ToString();
            var altUid = Guid.NewGuid().ToString();
            var doc = new Document { Uid = uid };
            var diffDoc = new Document { Uid = altUid };
            var anonDoc = new { Uid = uid };

            yield return new object[]
            {
                uid,
                doc,
                true
            };

            yield return new object[]
            {
                uid,
                diffDoc,
                false
            };

            yield return new object[]
            {
                uid,
                anonDoc,
                false
            };
        }

        [Theory]
        [MemberData(nameof(DocEqualityTests))]
        public void Equality_Is_Based_On_Type_And_Uid(
            string srcUid,
            object target,
            bool areEqual)
        {
            // arrange
            var refDoc = new Document { Uid = srcUid };

            // act
            var equal = refDoc.Equals(target);

            // assert
            Assert.Equal(areEqual, equal);
        }

        [Theory]
        [InlineData("one", "author", new string[0])]
        [InlineData("two", "author", new[] { "one" })]
        [InlineData("three", "author", new[] { "one", "two" })]
        public void ToString_Contains_Uid_Alias_And_TagCount(
            string uid,
            string alias,
            string[] tags)
        {
            // arrange
            var doc = new Document
            {
                Uid = uid,
                AuthorAlias = alias,
            };

            doc.Tags.AddRange(tags);

            // act
            var str = doc.ToString();

            // assert
            Assert.Contains(uid, str);
            Assert.Contains(alias, str);
            Assert.Contains(tags.Length.ToString(), str);
        }
    }
}
