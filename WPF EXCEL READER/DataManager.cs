using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPF_EXCEL_READER
{

    public class DataManager : INotifyPropertyChanged
    {
        public ObservableCollection<Customer> customers { get; } = new ObservableCollection<Customer>();

        private ObservableCollection<SaveFile> saveFiles = new ObservableCollection<SaveFile>();
        public ObservableCollection<SaveFile> SaveFiles { get { return saveFiles; } set { saveFiles = value;  NotifyPropertyChanged("SaveFiles"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public int GetIndexFromPath(string path)
        {
            for(int i = 0; i < saveFiles.Count; i++)
            {
                if (saveFiles[i].Path.Equals(path))
                    return i;
            }
            return 0;
        }
        public string GetPathFromIndex(int ind)
        {
            if (ind < saveFiles.Count && ind >= 0)
                return SaveFiles[ind].Path;
            return "";
        }

        public void SwitchToSaveFile(int ind)
        {
            if(ind < saveFiles.Count && ind >= 0)
              SwitchCustomerListToCurrentSaveFile(saveFiles[ind]);
        }
        public void AddSaveFile(SaveFile s)
        {
            this.saveFiles.Add(s);
            SwitchCustomerListToCurrentSaveFile(s);
            Console.WriteLine(saveFiles.Count + "  " + s.Id + "  " + s.Path);
        }

        public void SwitchCustomerListToCurrentSaveFile(SaveFile s)
        {
            customers.Clear();
            AddCustomer(SaveLoader.ReadSave(s.Path));
        }

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


        public string GetCustomerListCount()
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
