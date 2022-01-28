using System;
using PlanetaryDocs.Domain;
using Xunit;

namespace DomainTests
{
    public class ValidationStateTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToString_Includes_State_And_Message(
            bool isValid)
        {
            // arrange
            var state = new ValidationState
            {
                IsValid = isValid,
                Message = Guid.NewGuid().ToString()
            };

            // act
            var str = state.ToString();

            // assert
            if (isValid)
            {
                Assert.Contains("Valid", str);
            }
            else
            {
                Assert.Contains("Invalid", str);
                Assert.Contains(state.Message, str);
            }
        }
    }
}
