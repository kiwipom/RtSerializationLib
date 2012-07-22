namespace RtSerializationLib.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(string serializedString);
        string Serialize<T>(T obj);
    }
}