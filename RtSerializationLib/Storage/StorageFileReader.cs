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
            where T : class
        {
            // grab file from filename
            IBuffer buffer = await LoadBufferAsync(filename, fileNotFoundAction);

            if (buffer == null || buffer.Length == 0)
                return null;

            // turn file into string (including encryption if necessary)
            var serializedString = await _encryptionService.CreateStringFromBuffer(buffer);

            return _serializer.Deserialize<T>(serializedString);
        }


        private async Task<IBuffer> LoadBufferAsync(string filename, Action<FileNotFoundException> fileNotFoundAction)
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);

                using (var stream = await storageFile.OpenReadAsync())
                {
                    using (var readStream = stream.GetInputStreamAt(0))
                    {
                        var reader = new DataReader(readStream);
                        uint fileLength = await reader.LoadAsync((uint)stream.Size);
                        var stringContent = reader.ReadString(fileLength);

                        return System.Text.Encoding.UTF8.GetBytes(stringContent).AsBuffer();
                    }
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