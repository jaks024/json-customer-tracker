using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Customer_Tracker
{

    public class DataManager : INotifyPropertyChanged
    {
        public SortManager sortManager = new SortManager();

        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get
            {
                if (isSearching)
                    return searchResultCustomers;
                return customers;
            }
            set
            {   if (isSearching)
                    searchResultCustomers = value;
                else
                    customers = value;
                NotifyPropertyChanged("Customers");
            }
        }

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


        public void Sort()
        {
            if(!sortManager.sorted)
                Customers = sortManager.SortByNumber(Customers);
        }

        #region Search
        private bool isSearching = false;
        private ObservableCollection<Customer> searchResultCustomers = new ObservableCollection<Customer>();

        public void UpdateEditedItemInSearch(Customer c, int mainInd, int searchInd)
        {
            Customers[searchInd] = c;
            if (isSearching)
                customers[mainInd] = c;
        }
        public void ResetSearch()
        {
            Customers = customers;
        }
        public void SetSearching(bool state)
        {
            isSearching = state;
        }

        public void Search(string terms)
        {
            if(terms.Length == 0)
            {
                Customers = customers;
            }
            bool initial = true;
            string[] t = terms.ToLower().Split(' ');
            ObservableCollection<Customer> result = new ObservableCollection<Customer>();
            for(int i = 0; i < t.Length; i++)
            {
                if (initial)
                {
                    for (int x = 0; x < customers.Count; x++)
                    {
                        if (customers[x].SearchTerm.Contains(t[i]))
                        {
                            result.Add(customers[x]);
                        }
                    }
                    if (result.Count == 0)
                    {
                        break;
                    }
                    initial = false;
                }
                else
                {
                    ObservableCollection<Customer> temp = new ObservableCollection<Customer>();
                    for (int x = 0; x < result.Count; x++)
                    {
                        if (result[x].SearchTerm.Contains(t[i]))
                        {
                            temp.Add(result[x]);
                        }
                    }
                    if (temp.Count == 0)
                    {
                        break;
                    }
                    result = temp;
                }
            }
            searchResultCustomers = result;
            Customers = searchResultCustomers;
            Console.WriteLine(searchResultCustomers.Count);
        }
        #endregion

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
            SaveLoader save = new SaveLoader();
            AddCustomer(save.ReadSave(s.Path));
        }

        public void AddCustomer(Customer c)
        {
            this.customers.Add(c);
            sortManager.SetSortedFalse();
        }
        public void AddCustomer(List<Customer> l)
        {
            if (l == null)
                return;

            foreach(Customer c in l)
            {
                this.customers.Add(c);
            }
            sortManager.SetSortedFalse();
        }

        public void RemoveCustomer(Customer c)
        {
            customers.Remove(c);
            sortManager.SetSortedFalse();
        }
        public void RemoveCustomer(List<Customer> cs)
        {
            foreach(Customer customer in cs)
            {
                customers.Remove(customer);
            }
            sortManager.SetSortedFalse();
        }

        internal void RemoveCustomer(IEnumerable<Customer> collection)
        {
            List<Customer> c = collection.ToList();
            foreach (Customer customer in c)
            {
                customers.Remove(customer);
                if (isSearching)
                    searchResultCustomers.Remove(customer);
            }
            sortManager.SetSortedFalse();
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
