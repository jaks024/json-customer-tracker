using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer_Tracker
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

        public string SearchTerm { get { return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", Id, FirstName, MiddleName, LastName, PhoneNumber, Street, City, Province, PostalCode, Comment, Type).ToLower(); } }

        public CustomerTypes Type { get;  set; }
    }
}
