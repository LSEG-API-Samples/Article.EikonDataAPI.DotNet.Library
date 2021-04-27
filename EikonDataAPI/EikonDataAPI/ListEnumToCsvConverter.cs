using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EikonDataAPI
{
    internal class ListEnumToCsvConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) return;
            var listEnum = (List<T>)value;
            List<string> listString = new List<string>();
            foreach (var e in listEnum)
            {
                listString.Add(Enum.GetName(typeof(T), e));
            }

            writer.WriteValue(string.Join(",", listString));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new List<string>(((string)reader.Value).Split(','));
        }
    }
}
