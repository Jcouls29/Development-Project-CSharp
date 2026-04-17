using System.Collections.Generic;
using Sparcpoint.Inventory.SqlServer.Internal;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class JsonListTests
    {
        [Fact]
        public void Serialize_Null_Returns_EmptyArray()
        {
            Assert.Equal("[]", JsonList.Serialize(null));
        }

        [Fact]
        public void Serialize_Then_Deserialize_RoundTrips()
        {
            var input = new List<string> { "a", "b", "c" };
            var json = JsonList.Serialize(input);
            var back = JsonList.Deserialize(json);
            Assert.Equal(input, back);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deserialize_NullOrBlank_ReturnsEmptyList(string input)
        {
            Assert.Empty(JsonList.Deserialize(input));
        }

        [Fact]
        public void Deserialize_Malformed_ReturnsEmptyList_WithoutThrowing()
        {
            // EVAL: Comportamiento defensivo documentado — legacy data mal-formado no rompe el flujo.
            Assert.Empty(JsonList.Deserialize("{not-json"));
        }
    }
}
