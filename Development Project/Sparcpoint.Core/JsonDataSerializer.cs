using Newtonsoft.Json;
using System;

namespace Sparcpoint
{
    public class JsonDataSerializer : IDataSerializer
    {
        public object Deserialize(Type dataType, string value)
        {
            return JsonConvert.DeserializeObject(value, dataType);
        }

        public string Serialize(object model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
