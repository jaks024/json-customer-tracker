using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace Customer_Tracker
{
    public class SortManager
    {
        public bool descending = false;
        public bool sorted = false;
        public ObservableCollection<Customer> SortByNumber(ObservableCollection<Customer> list)
        {
            sorted = true;
            if (descending)
            {
                ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
                List<Customer> temp = list.OrderByDescending(c => c.Id).ToList();
                foreach(Customer c in temp)
                {
                    customers.Add(c);
                }
                return customers;
            }
            else
            {
                ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
                List<Customer> temp = list.OrderBy(c => c.Id).ToList();
                foreach (Customer c in temp)
                {
                    customers.Add(c);
                }
                return customers;
            }
        }

        public void SetSortedFalse()
        {
            sorted = false;
        }
    }
}
