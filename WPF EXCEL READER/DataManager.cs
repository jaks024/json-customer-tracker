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
        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers { get { return customers; } set { customers = value; NotifyPropertyChanged("Customers"); } }

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

        #region Save Files
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
                return saveFiles[ind].Path;
            return "";
        }
        public void RemoveSaveFromPath(string path)
        {
            saveFiles.RemoveAt(GetIndexFromPath(path));
        }
        public SaveFile GetNextAvailableSaveFile()
        {
            return saveFiles[0];
        }
        public int GetSaveFileCount()
        {
            return saveFiles.Count;
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

        #endregion

        #region Customers

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

        public void RemoveCustomer(Customer c)
        {
            customers.Remove(c);
        }
        public void RemoveCustomer(List<Customer> cs)
        {
            foreach(Customer customer in cs)
            {
                customers.Remove(customer);
            }
        }

        internal void RemoveCustomer(IEnumerable<Customer> collection)
        {
            List<Customer> c = collection.ToList();
            foreach (Customer customer in c)
            {
                customers.Remove(customer);
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


        public int GetCustomerListCount()
        {
            return customers.Count;
        }
        #endregion

    }
}
