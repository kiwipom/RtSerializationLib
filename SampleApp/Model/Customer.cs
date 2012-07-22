using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp.Model
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Address PostalAddress { get; set; }
        public Address PhysicalAddress { get; set; }
        public int Status { get; set; }
    }
}
