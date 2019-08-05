using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace WPF_EXCEL_READER
{


    public class DataManager
    {
        private static ObservableCollection<Customer> customers = new ObservableCollection<Customer>();

        public static void AddCustomer(Customer c)
        {
            customers.Add(c);
        }
        public static void AddCustomer(List<Customer> l)
        {
            foreach(Customer c in l)
            {
                customers.Add(c);
            }     
        }
        public static ObservableCollection<Customer> GetAllCustomers()
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
