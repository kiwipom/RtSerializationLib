using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace RtSerializationLib.Encryption
{
    public class UnencryptedService : IEncryptionService
    {
        public async Task<string> CreateStringFromBuffer(IBuffer buffer)
        {
            var bytes = buffer.ToArray();
            return await Task.FromResult(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        public async Task<IBuffer> CreateBufferFromString(string serializedString)
        {
            var bytes = Encoding.UTF8.GetBytes(serializedString);
            return await Task.FromResult(bytes.AsBuffer());
        }
    }
}