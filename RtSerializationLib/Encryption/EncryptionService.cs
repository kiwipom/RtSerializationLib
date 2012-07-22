using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using RtSerializationLib.Serialization;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;

namespace RtSerializationLib.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        private readonly string _descriptor;
        private readonly BinaryStringEncoding _encoding;

        private EncryptionService(string descriptor, BinaryStringEncoding encoding)
        {
            _encoding = encoding;
            _descriptor = descriptor;
        }

        public static IEncryptionService Create(uint bufferSize = 0, 
            string descriptor = "LOCAL=user",
            BinaryStringEncoding encoding = BinaryStringEncoding.Utf16BE)
        {
            return new EncryptionService(descriptor, encoding);
        }

        
        public async Task<string> CreateStringFromBuffer(IBuffer buffer)
        {
            var provider = new DataProtectionProvider();

            var inputData = new InMemoryRandomAccessStream();
            var unprotectedData = new InMemoryRandomAccessStream();

            // Retrieve an IOutputStream object and fill it with the input (encrypted) data.
            using (var outputStream = inputData.GetOutputStreamAt(0))
            {
                using (var writer = new DataWriter(outputStream))
                {
                    //  Write the contents of the buffer to the output stream
                    writer.WriteBuffer(buffer);
                    await writer.StoreAsync();
                    await outputStream.FlushAsync();
                }

                using (var source = inputData.GetInputStreamAt(0))
                {
                    using (var dest = unprotectedData.GetOutputStreamAt(0))
                    {
                        await provider.UnprotectStreamAsync(source, dest);
                        await dest.FlushAsync();
                    }
                }
            }

            // Write the decrypted data to an IBuffer object.
            using (var reader = new DataReader(unprotectedData.GetInputStreamAt(0)))
            {
                await reader.LoadAsync((uint)unprotectedData.Size);
                var buffUnprotectedData = reader.ReadBuffer((uint)unprotectedData.Size);

                return CryptographicBuffer.ConvertBinaryToString(_encoding, buffUnprotectedData);
            }
        }

        public async Task<IBuffer> CreateBufferFromString(string serializedString)
        {
            var provider = new DataProtectionProvider(_descriptor);
            var buffer = CryptographicBuffer.ConvertStringToBinary(serializedString, _encoding);

            using (var inputData = new InMemoryRandomAccessStream())
            {
                using (var encryptedData = new InMemoryRandomAccessStream())
                {
                    using (var outputStream = inputData.GetOutputStreamAt(0))
                    {
                        using (var writer = new DataWriter(outputStream))
                        {
                            writer.WriteBuffer(buffer);
                            await writer.StoreAsync();
                            await outputStream.FlushAsync();
                        }

                        using (var source = inputData.GetInputStreamAt(0))
                        {
                            using (var dest = encryptedData.GetOutputStreamAt(0))
                            {
                                await provider.ProtectStreamAsync(source, dest);
                                await dest.FlushAsync();
                            }
                        }
                    }

                    //Verify that the protected data does not match the original
                    var inputBuffer = await CreateBuffer(inputData);
                    var encryptedBuffer = await CreateBuffer(encryptedData);

                    if (CryptographicBuffer.Compare(inputBuffer, encryptedBuffer))
                        throw new Exception("ProtectStreamAsync returned unprotected data");

                    return encryptedBuffer;
                }
            }
        }


        private async Task<IBuffer> CreateBuffer(IRandomAccessStream stream)
        {
            var reader = new DataReader(stream.GetInputStreamAt(0));

            var size = (uint) stream.Size;
            await reader.LoadAsync(size);

            return reader.ReadBuffer(size);
        }
    }



}