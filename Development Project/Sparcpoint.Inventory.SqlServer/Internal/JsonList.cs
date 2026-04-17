using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sparcpoint.Inventory.SqlServer.Internal
{
    /// <summary>
    /// EVAL: Internal helper to serialize/deserialize columns the DB
    /// stores as VARCHAR(MAX) (ProductImageUris, ValidSkus). Centralized
    /// here so that if we switch to JSON-native (OPENJSON) only one file changes.
    /// </summary>
    internal static class JsonList
    {
        public static string Serialize(IEnumerable<string> values)
        {
            if (values == null) return "[]";
            return JsonConvert.SerializeObject(values);
        }

        public static IList<string> Deserialize(string serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized)) return new List<string>();
            // EVAL: If data is malformed (legacy) we return an empty list instead of throwing.
            try
            {
                return JsonConvert.DeserializeObject<List<string>>(serialized) ?? new List<string>();
            }
            catch (JsonException)
            {
                return new List<string>();
            }
        }
    }
}
