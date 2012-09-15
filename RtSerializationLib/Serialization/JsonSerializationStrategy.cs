using Newtonsoft.Json;

namespace RtSerializationLib.Serialization
{
    public class JsonSerializer : ISerializer
    {


        public T Deserialize<T>(string serializedString)
        {
            return JsonConvert.DeserializeObject<T>(serializedString);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                );
        }
    }
}