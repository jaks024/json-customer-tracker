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
            typeComboBox.SelectedIndex = 0;
        }

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
            }
        }

        //clear path and all item in DataManager
        private void BtnClearFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFile save = (SaveFile)pathComboBox.SelectedItem;
            MessageBox.Show(save.Path);
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

        private void PathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dm.SwitchToSaveFile(pathComboBox.SelectedIndex);
            currentSavePath = dm.GetPathFromIndex(pathComboBox.SelectedIndex);
        }

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
            c.PostalCode = provinceTextBox.Text;
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

        private void SaveToOpenedFile(object sender, RoutedEventArgs e)
        {
            SaveLoader.WriteToExistingFile(currentSavePath, dm);
        }

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
    }
}
