using MetroIoc;
using RtSerializationLib.Encryption;
using RtSerializationLib.Serialization;

namespace SampleApp
{
    public static class Ioc
    {
        private static IContainer _container;

        public static void Configure()
        {
            var encryptionService = EncryptionService.Create(10000);

            _container = new MetroContainer()
                .Register<ISerializer, JsonSerializer>()
                .Register<IEncryptionService, UnencryptedService>("unencrypted")
                .RegisterInstance<IEncryptionService>(encryptionService);
        }

        public static T Resolve<T>(string key = null)
        {
            return _container.Resolve<T>(key);
        }
    }
}