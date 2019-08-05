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
        public ObservableCollection<Customer> customers { get; } = new ObservableCollection<Customer>();

        public void AddCustomer(Customer c)
        {
            this.customers.Add(c);
        }
        public void AddCustomer(List<Customer> l)
        {
            foreach(Customer c in l)
            {
                this.customers.Add(c);
            }     
        }
        public ObservableCollection<Customer> GetAllCustomers()
        {
            return customers;
        }

        public void ClearDataPresent()
        {
            customers.Clear();
        }


        public string GetListCount()
        {
            return customers.Count.ToString();
        }
        public string GetCustomersToString()
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
