using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using RtSerializationLib.Encryption;
using RtSerializationLib.Serialization;
using RtSerializationLib.Storage;
using SampleApp.Model;

namespace SampleApp
{
    public class ViewModel
    {
        private readonly ISerializer _serializer;
        private readonly ICommand _loadCommand;
        private readonly ICommand _saveCommand;
        private readonly IEncryptionService _unencryptedService;
        private readonly IEncryptionService _encryptedService;
        private bool _isEncrypted;

        public ViewModel(ISerializer serializer)
        {
            _serializer = serializer;
            _unencryptedService = new UnencryptedService();
            _encryptedService = new UnencryptedService();

            _loadCommand = new RelayCommand(LoadAction);
            _saveCommand = new RelayCommand(SaveAction);

            CreateCustomer();
        }

        private IEncryptionService EncryptionService
        {
            get { return _isEncrypted ? _encryptedService : _unencryptedService; }
        }


        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set { _isEncrypted = value; }
        }

        private void SaveAction()
        {
            var writer = new StorageFileWriter(EncryptionService, _serializer);
            writer.WriteDataAsync(Customer, "customer.json");
        }

        private void LoadAction()
        {
            throw new System.NotImplementedException();
        }

        public ICommand LoadCommand { get { return _loadCommand; } }
        public ICommand SaveCommand { get { return _saveCommand; } }

        private async void LoadCustomer()
        {
            var reader = new StorageFileReader(Ioc.Resolve<IEncryptionService>(), Ioc.Resolve<ISerializer>());

            //Customer = await reader.LoadDataAsync<Customer>("customer.json");
            Customer = await reader.LoadDataAsync<Customer>("customer.json.enc");
        }

        private void CreateCustomer()
        {
            Customer = new Customer
            {
                Id = 435435,
                Name = "John Blokey-Guy",
                PhoneNumber = "09 111-2222",
                Status = 9,
                PhysicalAddress = new Address
                {
                    Number = "100",
                    Street = "High Street",
                    Suburb = "CBD",
                    City = "Auckland",
                    PostCode = "1010",
                    Country = "New Zealand"
                }
            };
        }

        public Customer Customer { get; set; }
    }
}