using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_EXCEL_READER
{
    class DataManager
    {
        private static List<Customer> customers = new List<Customer>();

        public static void AddCustomer(Customer c)
        {
            customers.Add(c);
        }
        public static void AddCustomer(List<Customer> l)
        {
            customers.AddRange(l);
        }
        public static List<Customer> GetAllCustomers()
        {
            return customers;
        }

        public static void ClearDataPresent()
        {
            customers.Clear();
        }


        public static string GetListCount()
        {
            return customers.Count.ToString();
        }
        public static string GetCustomersToString()
        {
            string s = "";
            foreach(Customer c in customers)
            {
                s += string.Format("Id: {0}, Name: {1}, PhoneNumber: {2}, Address: {3}", c.Id, c.Name, c.PhoneNumber, c.Address);
            }
            return s;
        }


    }
}
