using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Text.RegularExpressions;
namespace WPF_EXCEL_READER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataManager dm = new DataManager();
        public string currentSavePath = "";
        public MainWindow()
        {
            InitializeComponent();
            CustomerListBox.DataContext = dm;
            pathComboBox.DataContext = dm;

            typeComboBox.ItemsSource = Enum.GetValues(typeof(CustomerTypes)).Cast<CustomerTypes>();
            typeEditComboBox.ItemsSource = Enum.GetValues(typeof(CustomerTypes)).Cast<CustomerTypes>();
            typeComboBox.SelectedIndex = 0;
            typeEditComboBox.SelectedIndex = 0;
            TabMenusUpdateOnState(false);
        }

        #region File open, save, and clear
        //load json save file into DataManager
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                pathComboBox.Text = openFileDialog.FileName;
                currentSavePath = openFileDialog.FileName;

                dm.AddSaveFile(new SaveFile() { Id = dm.SaveFiles.Count, Path = currentSavePath });
                pathComboBox.SelectedItem = dm.SaveFiles[dm.GetIndexFromPath(currentSavePath)];

                TabMenusUpdateOnState(true);
            }
        }

        //clear path and all item in DataManager
        private void BtnClearFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFile save = (SaveFile)pathComboBox.SelectedItem;
            MessageBox.Show("Cleared " + save.Path);
            dm.RemoveSaveFromPath(save.Path);      
            pathComboBox.Text = "";
            dm.ClearDataPresent();
            if(dm.GetSaveFileCount() > 0)
            {
                SaveFile s = dm.GetNextAvailableSaveFile();
                pathComboBox.SelectedItem = s;
                currentSavePath = s.Path; 
            }
        }
        private void SaveToOpenedFile(object sender, RoutedEventArgs e)
        {
            SaveLoader.WriteToExistingFile(currentSavePath, dm);
        }

        private void PathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dm.SwitchToSaveFile(pathComboBox.SelectedIndex);
            currentSavePath = dm.GetPathFromIndex(pathComboBox.SelectedIndex);
        }

        #endregion


        #region info form
        private static readonly Regex numberRegex = new Regex("[^0-9]");
        private void NumberOnlyTextBoxValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = numberRegex.IsMatch(e.Text);
        }

        private void AddNewCustomerOnClick(object sender, RoutedEventArgs e)
        {
            Customer c = new Customer();
            c.FirstName = nameFirstTextBox.Text;
            c.MiddleName = nameMiddleTextBox.Text;
            c.LastName = nameLastTextBox.Text;
            c.Street = streetTextBox.Text;
            c.City = cityTextBox.Text;
            c.Province = provinceTextBox.Text;
            c.PostalCode = postalTextBox.Text;
            c.PhoneNumber = phoneNumberTextBox.Text;
            c.Type = (CustomerTypes)typeComboBox.SelectedIndex;
            c.Comment = commentTextBox.Text;
            try
            {
                c.Id = int.Parse(idTextBox.Text);
            } catch
            {
                MessageBox.Show("ID Cannot be empty");
                return;
            }
            

            dm.AddCustomer(c);
            ClearTextBoxInputs();
        }

        private void ClearNewCustomerFieldsOnClick(object sender, RoutedEventArgs e)
        {
            ClearTextBoxInputs();
        }

        private void ClearTextBoxInputs()
        {
            nameFirstTextBox.Clear();
            nameMiddleTextBox.Clear();
            nameLastTextBox.Clear();
            streetTextBox.Clear();
            cityTextBox.Clear();
            provinceTextBox.Clear();
            postalTextBox.Clear();
            phoneNumberTextBox.Clear();
            typeComboBox.SelectedIndex = 0;
            commentTextBox.Clear();
            idTextBox.Clear();
        }

        private void AutoIdOnClick(object sender, RoutedEventArgs e)
        {
            if (dm.GetCustomerListCount() == 0)
            {
                idTextBox.Text = "0";
                return;
            }
            int largest = 0;
            foreach(Customer c in dm.GetAllCustomers())
            {
                if (c.Id > largest)
                    largest = c.Id;
            }
            idTextBox.Text = (largest + 1).ToString();
        }
        #endregion


        #region list editing

        private void DeleteSelectedOnClick(object sender, RoutedEventArgs e)
        {
            if (CustomerListBox.Items.Count == 0 || CustomerListBox.SelectedItems.Count == 0)
                return;
            else
            {
                System.Collections.IList items = (System.Collections.IList)CustomerListBox.SelectedItems;
                var collection = items.Cast<Customer>();
                dm.RemoveCustomer(collection);
            }
        }

        #endregion

        //add dynamic search
        #region search
        private bool started = false;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int searchWaitDuration = 1;
        private bool first = false;
        private void StartDispatcherTimer()
        {
            if (!first)
            {
                dispatcherTimer.Tick += new EventHandler(SearchTimerTick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, searchWaitDuration);
                first = true;
            }
            dispatcherTimer.Start();
            started = true;
        }

        private void SearchTimerTick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            started = false;
            dm.ClearDataPresent();
            dm.Search(searchTextBox.Text);
            Console.WriteLine(searchTextBox.Text);
        }
        private void SearchTextboxInput(object sender, TextCompositionEventArgs e)
        {
            if (!started)
            {
                StartDispatcherTimer();
                Console.WriteLine("started");
            }
        }

        private void SearchOnClick(object sender, RoutedEventArgs e)
        {
            dm.Search(searchTextBox.Text);
        }

        private void TabMenusUpdateOnState(bool state)
        {
            searchTab.IsEnabled = state;
            editTab.IsEnabled = state;
        }

        private void TabMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tabMenus.SelectedIndex)
            {
                default:
                    {
                        inEdit = false;

                        dm.SetSearching(false);
                        ClearSearch();
                        pathComboBox.IsEnabled = true;
                        btnOpenFile.IsEnabled = true;
                        saveToFileButton.IsEnabled = true;
                        btnClearFile.IsEnabled = true;

                        CustomerListBox.SelectionMode = SelectionMode.Extended;
                        break;
                    }
                case 1:
                    {
                        inEdit = false;

                        dm.SetSearching(true);
                        pathComboBox.IsEnabled = false;
                        btnOpenFile.IsEnabled = false;
                        saveToFileButton.IsEnabled = false;
                        btnClearFile.IsEnabled = false;

                        CustomerListBox.SelectionMode = SelectionMode.Extended;
                        break;
                    }
                case 2:
                    {
                        inEdit = true;
                        UpdateEditField();
                        pathComboBox.IsEnabled = false;
                        btnOpenFile.IsEnabled = false;
                        saveToFileButton.IsEnabled = false;
                        btnClearFile.IsEnabled = false;

                        CustomerListBox.SelectionMode = SelectionMode.Single;
                        break;
                    }
            }
        }

        private void ClearSearchOnClick(object sender, RoutedEventArgs e)
        {
            ClearSearch();
        }

        private void ClearSearch()
        {
            searchTextBox.Clear();
            dm.ResetSearch();
        }
        #endregion


        private bool inEdit = false;
        private void UpdateEditField()
        {
            if (inEdit && CustomerListBox.SelectedItem != null)
            {
                FillEditFieldSelected((Customer)CustomerListBox.SelectedItem);
            }
        }
        private void FillEditFieldSelected(Customer c)
        {
            nameFirstEditTextBox.Text = c.FirstName;
            nameMiddleEditTextBox.Text = c.MiddleName;
            nameLastEditTextBox.Text = c.LastName;
            streetEditTextBox.Text = c.Street;
            cityEditTextBox.Text = c.City;
            provinceEditTextBox.Text = c.Province;
            postalEditTextBox.Text = c.PostalCode;
            phoneNumberEditTextBox.Text = c.PhoneNumber;
            typeEditComboBox.SelectedIndex = (int)c.Type;
            commentEditTextBox.Text = c.Comment;
            idEditTextBox.Text = c.Id.ToString();
        }

        private void UpdateEditOnClick(object sender, RoutedEventArgs e)
        {
            Customer c = dm.Customers[CustomerListBox.SelectedIndex];
            c.FirstName = nameFirstEditTextBox.Text;
            c.MiddleName = nameMiddleEditTextBox.Text;
            c.LastName = nameLastEditTextBox.Text;
            c.Street = streetEditTextBox.Text;
            c.City = cityEditTextBox.Text;
            c.Province = provinceEditTextBox.Text;
            c.PostalCode = postalEditTextBox.Text;
            c.PhoneNumber = phoneNumberEditTextBox.Text;
            c.Type = (CustomerTypes)typeEditComboBox.SelectedIndex;
            c.Comment = commentEditTextBox.Text;
            try
            {
                c.Id = int.Parse(idEditTextBox.Text);
            }
            catch
            {
                MessageBox.Show("ID Cannot be empty");
                return;
            }
            dm.UpdateEditedItemInSearch(c, CustomerListBox.SelectedIndex);
            CustomerListBox.Items.Refresh();
            MessageBox.Show("Changes applied to ID: " + c.Id);
        }

        private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEditField();
        }
    }
}
