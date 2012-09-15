using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace RtSerializationLib.Serialization
{
    /// <summary>
    /// Json serializer using the built in methods
    /// </summary>
    public class JsonRtSerializationStrategy : ISerializer
    {

        public T Deserialize<T>(string serializedString)
        {
            return DeserializeFromString<T>(serializedString);
        }

        public string Serialize<T>(T obj)
        {
            return SerializeToString(obj);
        }



        /// <summary>
        /// Serializes an object to a string.
        /// </summary>
        /// <param name="objForSerialization">The object to serialize.</param>
        /// <returns>JSON string of the serialized object.</returns>
        public static string SerializeToString<T>(T objForSerialization)
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings
            {
                SerializeReadOnlyTypes = false
            });
            serializer.WriteObject(ms, objForSerialization);
            ms.Seek(0, SeekOrigin.Begin);
            string serialized = new StreamReader(ms).ReadToEnd();
            ms.Dispose();
            return serialized;
        }
        /// <summary>
        /// Deserializes a string into an object.
        /// </summary>
        /// <param name="serializedObject">The string to deserialize.</param>
        /// <param name="serializedObjectType">The output Type.</param>
        /// <returns>New instance of the read object.</returns>
        public static T DeserializeFromString<T>(string serializedObject)
        {
            TextReader tr = new StringReader(serializedObject);
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serializedObject));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return (T) serializer.ReadObject(ms);
        }
    }
}
