using System;

namespace Sparcpoint
{
    public interface IDataSerializer
    {
        string Serialize(object model);
        //T Deserialize<T>(string value);
        object Deserialize(Type dataType, string value);
    }

    public static class DataSerializerExtensions
    {
        public static string Serialize<T>(this IDataSerializer serializer, T model)
            => serializer.Serialize((object)model);
        public static T Deserialize<T>(this IDataSerializer serializer, string value)
            => (T)serializer.Deserialize(typeof(T), value);
    }
}
