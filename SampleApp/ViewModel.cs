using RtSerializationLib.Encryption;
using RtSerializationLib.Serialization;
using RtSerializationLib.Storage;
using SampleApp.Model;

namespace SampleApp
{
    public class ViewModel
    {
        public ViewModel()
        {
            //CreateCustomer();
            LoadCustomer();
        }

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

        public void SaveDetails()
        {
            var writer = new StorageFileWriter(new UnencryptedService(), new JsonSerializer());
            writer.WriteDataAsync(Customer, "customer.json");
        }
    }
}