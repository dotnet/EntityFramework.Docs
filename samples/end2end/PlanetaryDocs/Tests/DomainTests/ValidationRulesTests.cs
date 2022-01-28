using System;
using System.Collections.Generic;
using System.Linq;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class ValidationRulesTests
    {
        private const string Punctuation = "(),?!'\".";
        private const string BadPunctuation = "~^[]{},";
        private const string UidAllowed = "_.-";

        [Fact]
        public void ValidResult_Returns_IsValid_True()
        {
            // arrange and act
            var result = ValidationRules.ValidResult();

            // assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Invalid")]
        [InlineData("Data not good")]
        public void InvalidResult_Returns_Invalid_False_With_Message(
            string message)
        {
            // arrange and act
            var result = ValidationRules.InvalidResult(message);

            // assert
            Assert.False(result.IsValid);
            Assert.Equal(message, result.Message);
        }

        public static IEnumerable<object[]> CompoundTests()
        {
            var valid = ValidationRules.ValidResult();
            var invalid1 = ValidationRules.InvalidResult("bad");
            var invalid2 = ValidationRules.InvalidResult("not good");

            var matrix = new[]
            {
                (new [] { valid }, 0),
                (new [] { invalid1 }, 0),
                (new [] { valid, valid }, 0),
                (new [] { valid, invalid1 }, 1),
                (new [] { invalid1, valid }, 0),
                (new [] { invalid1, invalid2 }, 0),
                (new [] { valid, valid, valid }, 0),
                (new [] { valid, valid, invalid1 }, 2),
                (new [] { valid, invalid1, valid }, 1),
                (new [] { valid, invalid1, invalid2}, 1),
                (new [] { invalid1, valid, valid }, 0),
                (new [] { invalid1, valid, invalid2 }, 0),
                (new [] { invalid1, invalid1, valid }, 0)
            };

            foreach (var testCase in matrix)
            {
                yield return new object[]
                {
                    testCase.Item1,
                    testCase.Item2
                };
            }
        }

        [Theory]
        [MemberData(nameof(CompoundTests))]
        public void Compound_Result_Returns_First_Invalid(
            ValidationState[] states,
            int expectedIdx)
        {
            // arrange and act
            var expected = states[expectedIdx];
            var actual = ValidationRules.CompoundResult(
                nameof(ValidationRulesTests),
                nameof(ValidationRulesTests),
                states.Select(
                    state =>
                    (Func<string, string, ValidationState>)((_, __) => state)).ToArray());

            // assert
            if (expected.IsValid)
            {
                Assert.True(actual.IsValid);
            }
            else
            {
                Assert.Equal(expected.Message, actual.Message);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("x")]
        public void IsRequired_Disallows_Null_Or_WhiteSpace(
            string target)
        {
            // arrange
            var expected = !string.IsNullOrWhiteSpace(target);

            // act
            var actual = ValidationRules.IsRequired("field", target).IsValid;

            // assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("  ", false)]
        [InlineData(" a ", false)]
        [InlineData("a1", false)]
        [InlineData(" A ", false)]
        [InlineData("A1", false)]
        [InlineData("Excellent", true)]
        [InlineData("aaaa", true)]
        [InlineData("ZZZZ", true)]
        [InlineData("Exciting!", false)]
        public void IsAlphaOnly_Disallows_NonAlpha(
            string target,
            bool expected)
        {
            // arrange and act
            var state = ValidationRules.IsAlphaOnly("field", target);

            // assert
            Assert.Equal(expected, state.IsValid);
        }

        public static IEnumerable<object[]> SimpleTextCases()
        {
            var validText = new[]
            {
                "This is a simple title.",
                "This is exciting!",
                "Won't, you, be, my, neighbor?",
                "Word"
            };

            var validUid = new[]
            {
                "uid",
                "uid-two",
                "uid_two-three",
                "uid-is.here_score"
            };

            for (var uidFlag = 0; uidFlag < 2; uidFlag++)
            {
                var isUid = uidFlag == 1;
                var srcArray = isUid ? validUid : validText;
                var badUid = Punctuation.Union(BadPunctuation)
                    .Except(UidAllowed).ToArray();

                for (var idx = 0; idx < srcArray.Length; idx++)
                {
                    yield return new object[]
                    {
                        isUid, srcArray[idx], true
                    };

                    var badChoices = isUid ? badUid : BadPunctuation.ToCharArray();
                    var invalid = badChoices[idx % badChoices.Length];

                    yield return new object[]
                    {
                        isUid, $"{srcArray[idx]}{invalid}", false
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SimpleTextCases))]
        public void IsSimpleText_Disallows_Invalid_Punctuation(
            bool isUid,
            string target,
            bool expected)
        {
            // arrange and act
            var state = ValidationRules.IsSimpleText("field", target, isUid);

            // assert
            Assert.Equal(expected, state.IsValid);
        }

        public enum TestToCheck
        {
            IsRequired,
            IsAlpha,
            IsSimpleNoUid,
            IsSimpleUid
        };

        [Theory]
        [InlineData(TestToCheck.IsRequired)]
        [InlineData(TestToCheck.IsAlpha)]
        [InlineData(TestToCheck.IsSimpleNoUid)]
        [InlineData(TestToCheck.IsSimpleUid)]
        public void Invalid_Messages_Include_FieldName(
            TestToCheck testToCheck)
        {
            // arrange

            var field = testToCheck.ToString();
            ValidationState result = null;

            // act

            switch (testToCheck)
            {
                case TestToCheck.IsRequired:
                    result = ValidationRules.IsRequired(field, string.Empty);
                    break;

                case TestToCheck.IsAlpha:
                    result = ValidationRules.IsAlphaOnly(field, "!");
                    break;

                case TestToCheck.IsSimpleNoUid:
                    result = ValidationRules.IsSimpleText(field, BadPunctuation);
                    break;

                case TestToCheck.IsSimpleUid:
                    result = ValidationRules.IsSimpleText(field, BadPunctuation, true);
                    break;
            }

            // assert
            Assert.Contains(field, result.Message);
        }

        public static IEnumerable<object[]> PropertyMatrix()
        {
            var matrix = new[]
            {
                new object[]
                {
                    nameof(Document.AuthorAlias),
                    string.Empty,
                    false
                },
                new object[]
                {
                    nameof(Document.AuthorAlias),
                    "notgood!",
                    false
                },
                new object[]
                {
                    nameof(Document.AuthorAlias),
                    "good",
                    true
                },
                new object[]
                {
                    nameof(Document.Description),
                    string.Empty,
                    false
                },
                new object[]
                {
                    nameof(Document.Description),
                    "good as any description",
                    true
                },
                new object[]
                {
                    nameof(Document.Markdown),
                    string.Empty,
                    false
                },
                new object[]
                {
                    nameof(Document.Markdown),
                    "# The Spice must flow",
                    true
                },
                new object[]
                {
                    nameof(Document.Title),
                    string.Empty,
                    false
                },
                new object[]
                {
                    nameof(Document.Title),
                    "A title can't have ~ that",
                    false
                },
                new object[]
                {
                    nameof(Document.Title),
                    "A Sandworm was spotted!",
                    true
                },
                new object[]
                {
                    nameof(Document.Uid),
                    string.Empty,
                    false
                },
                new object[]
                {
                    nameof(Document.Uid),
                    "this doesn't work as a uid",
                    false
                },
                new object[]
                {
                    nameof(Document.Uid),
                    "perfect_uid-is-this",
                    true
                },
                new object[]
                {
                    "notafield",
                    "perfect",
                    false
                }
            };

            foreach (var testCase in matrix)
            {
                yield return testCase;
            }
        }

        [Theory]
        [MemberData(nameof(PropertyMatrix))]
        public void ValidateProperty_Applies_Correct_Validation(
            string fieldName,
            string value,
            bool isValid)
        {
            // arrange and act
            var validationState = ValidationRules.ValidateProperty(
                fieldName, value);

            // assert
            Assert.Equal(isValid, validationState.IsValid);
        }

        public static IEnumerable<object[]> DocumentMatrix()
        {
            const string uid = "uid";
            const string title = "Good title";
            const string alias = "dune";
            const string description = "Muad'Dib knows how to wear a stillsuite.";
            const string markdown = "# Jam On It";

            for (var docType = 0; docType < 7; docType++)
            {
                var document = docType == 6 ?
                    new Document() :
                    new Document
                    {
                        Uid = docType == 1 ? string.Empty : uid,
                        Title = docType == 2 ? "~/usr/home" : title,
                        AuthorAlias = docType == 3 ? "Muad'Dib" : alias,
                        Description = docType == 4 ? string.Empty : description,
                        Markdown = docType == 5 ? string.Empty : markdown,
                    };
                yield return new object[]
                {
                document,
                docType == 0
                };
            }
        }

        [Theory]
        [MemberData(nameof(DocumentMatrix))]
        public void ValidateDocument_Validates_All_Fields(
            Document doc,
            bool isValid)
        {
            // arrange and act
            var state = ValidationRules.ValidateDocument(doc);

            // assert
            if (isValid)
            {
                Assert.True(state.All(s => s.IsValid));
            }
            else
            {
                Assert.Contains(state, s => !s.IsValid);
            }
        }
    }
}

