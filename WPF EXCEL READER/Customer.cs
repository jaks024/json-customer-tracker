using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_EXCEL_READER
{
    public enum CustomerTypes { New, Regular, Old, VIP, Special };

    public struct Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }

        public string Comment { get; set; }

        public CustomerTypes Type { get;  set; }
    }
}
