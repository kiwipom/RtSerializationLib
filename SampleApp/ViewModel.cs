using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RtSerializationLib.Encryption;
using RtSerializationLib.Serialization;
using RtSerializationLib.Storage;
using SampleApp.Model;

namespace SampleApp
{
    public class ViewModel : ViewModelBase
    {
        private readonly ISerializer _serializer;
        private readonly ICommand _loadCommand;
        private readonly ICommand _saveCommand;
        private readonly IEncryptionService _unencryptedService;
        private readonly IEncryptionService _encryptedService;
        private bool _isEncrypted;
        private Customer _customer;

        public ViewModel(ISerializer serializer)
        {
            _serializer = serializer;
            _unencryptedService = new UnencryptedService();
            _encryptedService = EncryptionService.Create();

            _loadCommand = new RelayCommand(LoadAction);
            _saveCommand = new RelayCommand(SaveAction);
        }

        private IEncryptionService CurrentEncryptionService
        {
            get { return _isEncrypted ? _encryptedService : _unencryptedService; }
        }


        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set { _isEncrypted = value; }
        }

        public ICommand LoadCommand { get { return _loadCommand; } }
        public ICommand SaveCommand { get { return _saveCommand; } }

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                if (value == _customer) return;
                _customer = value;
                base.RaisePropertyChanged(() => Customer);
            }
        }

        private string Filename
        {
            get
            {
                var name = "customer.json";
                if (IsEncrypted)
                    name += ".enc";
                return name;
            }
        }

        private void SaveAction()
        {
            var writer = new StorageFileWriter(CurrentEncryptionService, _serializer);
            writer.WriteDataAsync(Customer, Filename);
        }

        private async void LoadAction()
        {
            try
            {
                var reader = new StorageFileReader(CurrentEncryptionService, _serializer);
                Customer = await reader.LoadDataAsync<Customer>(Filename);
            }
            catch (FileNotFoundException)
            {
                // file not found.
            }
        }

    }
}