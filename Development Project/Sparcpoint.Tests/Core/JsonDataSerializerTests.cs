using Xunit;

namespace Sparcpoint.Tests.Core
{
    public class JsonDataSerializerTests
    {
        private readonly JsonDataSerializer _Serializer = new JsonDataSerializer();

        // ── Serialize ─────────────────────────────────────────────────────────

        [Fact]
        public void Serialize_Object_ReturnsJsonString()
        {
            var result = _Serializer.Serialize(new { Name = "Widget", Count = 5 });

            Assert.Contains("\"Name\"", result);
            Assert.Contains("\"Widget\"", result);
            Assert.Contains("5", result);
        }

        [Fact]
        public void Serialize_Null_ReturnsNullLiteral()
        {
            var result = _Serializer.Serialize(null);

            Assert.Equal("null", result);
        }

        [Fact]
        public void Serialize_StringArray_ReturnsJsonArray()
        {
            var result = _Serializer.Serialize(new[] { "a", "b", "c" });

            Assert.Equal("[\"a\",\"b\",\"c\"]", result);
        }

        // ── Deserialize ───────────────────────────────────────────────────────

        [Fact]
        public void Deserialize_ValidJson_ReturnsTypedObject()
        {
            var json = "{\"Name\":\"Widget\",\"Count\":5}";

            var result = _Serializer.Deserialize<TestModel>(json);

            Assert.Equal("Widget", result.Name);
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void Deserialize_StringArray_ReturnsArray()
        {
            var json = "[\"a\",\"b\",\"c\"]";

            var result = _Serializer.Deserialize<string[]>(json);

            Assert.Equal(3, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("c", result[2]);
        }

        // ── Round-trip ────────────────────────────────────────────────────────

        [Fact]
        public void RoundTrip_SerializeThenDeserialize_PreservesValues()
        {
            var original = new TestModel { Name = "Round-Trip", Count = 42 };

            var json = _Serializer.Serialize<TestModel>(original);
            var result = _Serializer.Deserialize<TestModel>(json);

            Assert.Equal(original.Name, result.Name);
            Assert.Equal(original.Count, result.Count);
        }

        // ── Extension methods ─────────────────────────────────────────────────

        [Fact]
        public void SerializeExtension_WorksLikeDirectCall()
        {
            var model = new TestModel { Name = "Ext", Count = 1 };

            string direct    = _Serializer.Serialize((object)model);
            string extension = _Serializer.Serialize<TestModel>(model);

            Assert.Equal(direct, extension);
        }

        private class TestModel
        {
            public string Name  { get; set; } = string.Empty;
            public int    Count { get; set; }
        }
    }
}
