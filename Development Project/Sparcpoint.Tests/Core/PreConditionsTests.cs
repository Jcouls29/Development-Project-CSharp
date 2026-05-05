using System;
using Xunit;

namespace Sparcpoint.Tests.Core
{
    public class PreConditionsTests
    {
        // ── ParameterNotNull ──────────────────────────────────────────────────

        [Fact]
        public void ParameterNotNull_WhenNull_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => PreConditions.ParameterNotNull(null, "myParam"));

            Assert.Equal("myParam", ex.ParamName);
        }

        [Fact]
        public void ParameterNotNull_WhenNotNull_DoesNotThrow()
        {
            // Should complete without exception
            PreConditions.ParameterNotNull("any value", "myParam");
            PreConditions.ParameterNotNull(42, "myParam");
            PreConditions.ParameterNotNull(new object(), "myParam");
        }

        // ── StringNotNullOrWhitespace ─────────────────────────────────────────

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void StringNotNullOrWhitespace_WhenNullOrWhitespace_ThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(
                () => PreConditions.StringNotNullOrWhitespace(value, "myParam"));
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("a")]
        [InlineData("  text  ")]
        public void StringNotNullOrWhitespace_WhenValid_DoesNotThrow(string value)
        {
            PreConditions.StringNotNullOrWhitespace(value, "myParam");
        }
    }
}
