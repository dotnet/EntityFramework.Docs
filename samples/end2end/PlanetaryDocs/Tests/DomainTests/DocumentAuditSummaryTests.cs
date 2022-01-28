using System;
using System.Collections.Generic;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class DocumentAuditSummaryTests
    {
        public static IEnumerable<object[]> DocAuditSummaryTests()
        {
            var resolvers = new (Func<object, string> resolver, bool fromAudit)[]
            {
                (obj => ((DocumentAudit)obj).Id.ToString(), true),
                (obj => ((DocumentAudit)obj).Uid, true),
                (obj => ((DocumentAudit)obj).Timestamp.ToString(), true),
                (obj => ((Document)obj).AuthorAlias, false),
                (obj => ((Document)obj).Title, false),
            };

            var auditResolvers = new Func<DocumentAuditSummary, string>[]
            {
                auditSummary => auditSummary.Id.ToString(),
                auditSummary => auditSummary.Uid,
                auditSummary => auditSummary.Timestamp.ToString(),
                auditSummary => auditSummary.Alias,
                auditSummary => auditSummary.Title,
            };

            for (var idx = 0; idx < resolvers.Length; idx++)
            {
                yield return new object[]
                {
                    resolvers[idx].resolver,
                    resolvers[idx].fromAudit,
                    auditResolvers[idx]
                };
            }
        }

        [Theory]
        [MemberData(nameof(DocAuditSummaryTests))]
        public void Properties_Match_Audit_Or_Doc(
            Func<object, string> resolver,
            bool fromAudit,
            Func<DocumentAuditSummary, string> auditResolver)
        {
            // arrange
            var doc = new Document
            {
                Uid = nameof(DocAuditSummaryTests),
                AuthorAlias = "system",
                Title = "Document title"
            };

            var audit = new DocumentAudit(doc);

            // act
            var auditSummary = new DocumentAuditSummary(audit);
            var expected = fromAudit ? resolver(audit) : resolver(doc);
            var actual = auditResolver(auditSummary);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HashCode_Is_HashCode_Of_Id()
        {
            // arrange
            var summary = new DocumentAuditSummary
            {
                Id = Guid.NewGuid()
            };

            // act
            var expected = summary.Id.GetHashCode();
            var actual = summary.GetHashCode();

            // assert
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> EqualityTests()
        {
            var guid = Guid.NewGuid();
            var sameGuidSameUid = new DocumentAuditSummary
            {
                Id = guid,
                Uid = guid.ToString()
            };

            var sameGuidDifferentUid = new DocumentAuditSummary
            {
                Id = guid,
                Uid = nameof(EqualityTests)
            };

            var differentGuid = new DocumentAuditSummary
            {
                Id = Guid.NewGuid()
            };

            var anonymous = new
            {
                Id = guid
            };

            yield return new object[]
            {
                guid,
                sameGuidSameUid,
                true
            };

            yield return new object[]
            {
                guid,
                sameGuidDifferentUid,
                true
            };

            yield return new object[]
            {
                guid,
                differentGuid,
                false
            };

            yield return new object[]
            {
                guid,
                anonymous,
                false
            };
        }

        [Theory]
        [MemberData(nameof(EqualityTests))]
        public void Equality_Based_On_Type_And_Id(
            Guid id,
            object target,
            bool areEqual)
        {
            // arrange
            var summary = new DocumentAuditSummary
            {
                Id = id
            };

            // act
            var equals = summary.Equals(target);

            // assert
            Assert.Equal(areEqual, equals);
        }

        [Fact]
        public void ToString_Includes_Id_And_Uid()
        {
            // arrange
            var summary = new DocumentAuditSummary
            {
                Id = Guid.NewGuid(),
                Uid = nameof(EqualityTests)
            };

            // act
            var str = summary.ToString();

            // assert
            Assert.Contains(summary.Id.ToString(), str);
            Assert.Contains(summary.Uid, str);
        }
    }
}
