using System.Threading.Tasks;
using RtSerializationLib.Serialization;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;

namespace RtSerializationLib.Encryption
{
    public interface IEncryptionService
    {
        Task<string> CreateStringFromBuffer(IBuffer buffer);

        Task<IBuffer> CreateBufferFromString(string serializedString);
    }
}
