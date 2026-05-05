using System;
using Xunit;

namespace Sparcpoint.Tests.Core
{
    public class ParameterRequiredExceptionTests
    {
        [Fact]
        public void Constructor_WithParameterName_SetsParameterNameAndFormatsMessage()
        {
            var ex = new ParameterRequiredException("userId");

            Assert.Equal("userId", ex.ParameterName);
            Assert.Contains("userId", ex.Message);
        }

        [Fact]
        public void Constructor_WithParameterNameAndMessage_IncludesBothInMessage()
        {
            var ex = new ParameterRequiredException("userId", "Must be a positive integer.");

            Assert.Equal("userId", ex.ParameterName);
            Assert.Contains("userId", ex.Message);
            Assert.Contains("Must be a positive integer.", ex.Message);
        }

        [Fact]
        public void Constructor_WithInnerException_SetsInnerException()
        {
            var inner = new InvalidOperationException("root cause");
            var ex = new ParameterRequiredException("userId", inner);

            Assert.Equal("userId", ex.ParameterName);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException_SetsAll()
        {
            var inner = new InvalidOperationException("root cause");
            var ex = new ParameterRequiredException("userId", "Bad value.", inner);

            Assert.Equal("userId", ex.ParameterName);
            Assert.Contains("userId", ex.Message);
            Assert.Contains("Bad value.", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void IsException_CanBeCaughtAsException()
        {
            void Throw() => throw new ParameterRequiredException("x");

            Assert.Throws<ParameterRequiredException>(Throw);
        }
    }
}
