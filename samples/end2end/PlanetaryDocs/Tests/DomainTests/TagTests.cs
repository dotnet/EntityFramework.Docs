using System;
using System.Collections.Generic;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class TagTests
    {
        [Fact]
        public void New_Instance_Initializes_Documents()
        {
            // arrange and act
            var tag = new Tag();

            // assert
            Assert.NotNull(tag.Documents);
        }

        [Fact]
        public void HashCode_Is_HashCode_Of_TagName()
        {
            // arrange
            var tag = new Tag
            {
                TagName = nameof(TagTests)
            };

            // act
            var expected = tag.TagName.GetHashCode();
            var actual = tag.GetHashCode();

            // assert
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> TagEqualityTests()
        {
            yield return new object[]
            {
                new Tag { TagName = nameof(TagTests) },
                true
            };

            yield return new object[]
            {
                new Tag { TagName = nameof(TagEqualityTests) },
                false
            };

            yield return new object[]
            {
                new { TagName = nameof(TagTests) },
                false
            };
        }

        [Theory]
        [MemberData(nameof(TagEqualityTests))]
        public void Equality_Is_Based_On_Type_And_TagName(
            object target,
            bool areEqual)
        {
            // arrange
            var tag = new Tag { TagName = nameof(TagTests) };

            // act
            var equals = tag.Equals(target);

            // assert
            Assert.Equal(areEqual, equals);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(99)]
        public void ToString_Includes_TagName_And_Documents_Count(int docCount)
        {
            // arrange
            var tag = new Tag
            {
                TagName = Guid.NewGuid().ToString()
            };

            for (var idx = 0; idx < docCount; idx++)
            {
                tag.Documents.Add(
                    new DocumentSummary
                    {
                        Uid = idx.ToString(),
                        Title = $"Title #{idx}",
                        AuthorAlias = "test"
                    });
            }

            // act
            var str = tag.ToString();

            // assert
            Assert.Contains(tag.TagName, str);
            Assert.Contains(docCount.ToString(), str);
        }
    }
}
