using System;
using System.Threading.Tasks;
using RtSerializationLib.Encryption;
using RtSerializationLib.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace RtSerializationLib.Storage
{
    public class StorageFileWriter
    {
        private readonly IEncryptionService _encryptionService;
        private readonly ISerializer _serializer;

        public StorageFileWriter(IEncryptionService encryptionService, ISerializer serializer)
        {
            _encryptionService = encryptionService;
            _serializer = serializer;
        }

        public async Task WriteDataAsync<T>(T item, string filename)
        {
            var serializedString = _serializer.Serialize(item);

            var buffer = await _encryptionService.CreateBufferFromString(serializedString);

            await WriteToDiskAsync(buffer, filename);
        }



        private async Task WriteToDiskAsync(IBuffer buffer, string filename)
        {
            var storage = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (var stream = await storage.OpenTransactedWriteAsync())
            {
                await stream.Stream.WriteAsync(buffer);
                await stream.CommitAsync();
            }
        }
    }
}