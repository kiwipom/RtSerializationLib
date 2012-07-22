using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using RtSerializationLib.Encryption;
using RtSerializationLib.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace RtSerializationLib.Storage
{
    public class StorageFileReader
    {
        private readonly IEncryptionService _encryptionService;
        private readonly ISerializer _serializer;
        private readonly uint _bufferSize;

        public StorageFileReader(IEncryptionService encryptionService, ISerializer serializer,
            uint bufferSize = 100000)
        {
            _encryptionService = encryptionService ?? new UnencryptedService();
            _serializer = serializer;
            if (bufferSize <= 0)
                throw new ArgumentException(@"bufferSize must be greater than 0");
            _bufferSize = bufferSize;
        }

        /// <summary>
        /// Load data from disk asynchronously
        /// </summary>
        /// <param name="filename">Name of the encrypted file in local storage to read</param>
        /// <param name="fileNotFoundAction">Optional action to perform if specified file is not found</param>
        /// <returns>Buffer containing the encrypted data</returns>
        public async Task<T> LoadDataAsync<T>(string filename, Action<FileNotFoundException> fileNotFoundAction = null)
        {
            // grab file from filename
            var buffer = await LoadBufferAsync(filename, fileNotFoundAction);

            // turn file into string (including encryption if necessary)
            var serializedString = await _encryptionService.CreateStringFromBuffer(buffer);

            return _serializer.Deserialize<T>(serializedString);
        }


        private async Task<IBuffer> LoadBufferAsync(string filename, Action<FileNotFoundException> fileNotFoundAction)
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);

                var bytes = new byte[_bufferSize];
                var buffer = bytes.AsBuffer();

                using (var stream = await storageFile.OpenReadAsync())
                {
                    return await stream.ReadAsync(buffer, _bufferSize - 1, InputStreamOptions.None);
                }
            }
            catch (FileNotFoundException exception)
            {
                if (fileNotFoundAction != null)
                    fileNotFoundAction(exception);
            }
            return null;
        }

    }
}