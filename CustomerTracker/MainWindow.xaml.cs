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
using System.IO;

namespace Customer_Tracker
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
                LoadSaveFile(openFileDialog.FileName);
            }
        }
        public void LoadSaveFile(string path)
        {
            pathComboBox.Text = path;
            currentSavePath = path;

            dm.AddSaveFile(new SaveFile() { Id = dm.SaveFiles.Count, Path = currentSavePath });

            if (dm.GetSaveFileCount() >= 1)
            {
				if (dm.GetCustomerListCount() != 0)
					ChangeAllFormState(true, false);
                initialLoaded = true;
            }

            pathComboBox.SelectedItem = dm.SaveFiles[dm.GetIndexFromPath(currentSavePath)];
			if (dm.GetCustomerListCount() == 0)
			{
				btnClearFile.IsEnabled = true;
				saveToFileButton.IsEnabled = true;
				pathComboBox.IsEnabled = true;
			}
		}

        //clear path and all item in DataManager
        private void BtnClearFile_Click(object sender, RoutedEventArgs e)
        {
            if (pathComboBox.IsEnabled)
            {
                SaveFile save = (SaveFile)pathComboBox.SelectedItem;
                MessageBox.Show("Cleared " + save.Path);
                dm.RemoveSaveFromPath(save.Path);
            }
			ClearImageListView();
            dm.ClearDataPresent();
            if(dm.GetSaveFileCount() > 0)
            {
                SaveFile s = dm.GetNextAvailableSaveFile();
                pathComboBox.SelectedItem = s;
                currentSavePath = s.Path; 
            }
            else
            {
                ChangeAllFormState(false, false);
            }
        }

        private void SaveToOpenedFile(object sender, RoutedEventArgs e)
        {
            SaveLoader s = new SaveLoader();
            s.WriteToExistingFile(currentSavePath, dm);
            if (!pathComboBox.IsEnabled)
                LoadSaveFile(s.lastSavePath);
        }

        private void PathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dm.SwitchToSaveFile(pathComboBox.SelectedIndex);
            currentSavePath = dm.GetPathFromIndex(pathComboBox.SelectedIndex);
			if (dm.GetCustomerListCount() == 0)
			{
				ChangeAllFormState(false, false);
				btnClearFile.IsEnabled = true;
				saveToFileButton.IsEnabled = true;
				pathComboBox.IsEnabled = true;
			}
			else
			{
				ChangeAllFormState(true, false);
			}
			ClearImageListView();
		}

        private void ChangeAllFormState(bool state, bool ex)
        {
            searchTab.IsEnabled = state;
            editTab.IsEnabled = state;
            saveToFileButton.IsEnabled = state;
            btnClearFile.IsEnabled = state;
            btnDeleteEntry.IsEnabled = state;
            sortOptionsComboBox.IsEnabled = state;
            initialLoaded = state;
            btnSort.IsEnabled = state;
            pathComboBox.IsEnabled = ex ? !state : state;
        }
        #endregion


        #region info form
        private static readonly Regex numberRegex = new Regex("[^0-9-]+");
        private void NumberOnlyTextBoxValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = numberRegex.IsMatch(e.Text);
        }

        private void AddNewCustomerOnClick(object sender, RoutedEventArgs e)
        {
            Customer c = new Customer();
            c.FirstName = nameFirstTextBox.Text.Length == 0 ? "First" : nameFirstTextBox.Text;
            c.MiddleName = nameMiddleTextBox.Text;
            c.LastName = nameLastTextBox.Text.Length == 0 ? "Last Name" : nameLastTextBox.Text;
            c.Street = streetTextBox.Text.Length == 0 ? "Street" : streetTextBox.Text;
            c.City = cityTextBox.Text.Length == 0 ? "City" : cityTextBox.Text;
            c.Province = provinceTextBox.Text.Length == 0 ? "Province" : provinceTextBox.Text;
            c.PostalCode = postalTextBox.Text.Length == 0 ? "Postal Code" : postalTextBox.Text;
            c.PhoneNumber = phoneNumberTextBox.Text.Length == 0 ? "x-xxx-xxx-xxxx" : phoneNumberTextBox.Text;
            c.Type = (CustomerTypes)typeComboBox.SelectedIndex;
            c.Comment = commentTextBox.Text.Length == 0 ? "No Comments" : commentTextBox.Text;
            try
            {
                c.Id = int.Parse(idTextBox.Text);
            } catch
            {
                MessageBox.Show("ID cannot be empty");
                return;
            }
			c.ImageFolder = imagePathTextbox.Text.Length == 0 ? "No Image Directory" : imagePathTextbox.Text;


			dm.AddCustomer(c);
            ClearTextBoxInputs();
            ChangeAllFormState(true, dm.SaveFiles.Count > 0 ? false : true);
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
			imagePathTextbox.Clear();
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

		private void OpenImageFolder(object sender, RoutedEventArgs e)
		{
			using(var fbd = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult result = fbd.ShowDialog();

				if(result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					if (inEdit)
						imagePathEditTextbox.Text = fbd.SelectedPath;
					else
						imagePathTextbox.Text = fbd.SelectedPath;
				}
			}
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
				if (dm.GetCustomerListCount() == 0 && dm.SaveFiles.Count == 0)
				{
					ChangeAllFormState(false, false);
					ClearImageListView();
				}
				else if (dm.GetCustomerListCount() == 0 && dm.SaveFiles.Count > 0)
				{
					ChangeAllFormState(false, false);
					btnClearFile.IsEnabled = true;
					saveToFileButton.IsEnabled = true;
					pathComboBox.IsEnabled = true;
					ClearImageListView();
				}
			}

		}

        #endregion

        //add dynamic search
        #region search

        private bool searched = false;

        private void SearchTextBoxTextChange(object sender, TextCompositionEventArgs e)
        {
            searched = false;
        }
        private void SearchOnClick(object sender, RoutedEventArgs e)
        {
            if(!searched &&  !string.IsNullOrWhiteSpace(searchTextBox.Text))
                dm.Search(searchTextBox.Text, out searched);
        }
        private void ClearSearchOnClick(object sender, RoutedEventArgs e)
        {
            ClearSearch();
            searched = false;
        }

        private void ClearSearch()
        {
            searchTextBox.Clear();
            dm.ResetSearch();
        }

        private void TabChangeFormStateChange(bool state, bool ex)
        {
            pathComboBox.IsEnabled = state;
            btnOpenFile.IsEnabled = state;
            saveToFileButton.IsEnabled = state;
            btnClearFile.IsEnabled = state;

            pathComboBox.IsEnabled = ex ? !state : state;
        }

        private bool initialLoaded = false;
        private void TabMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tabMenus.SelectedIndex)
            {
                default:
                    {
                        if (initialLoaded)
                        {
                            inEdit = false;
                            editFilled = false;
                            searched = false;

                            dm.SetSearching(false);
                            ClearSearch();
                            TabChangeFormStateChange(true, !pathComboBox.HasItems);
                            CustomerListBox.SelectionMode = SelectionMode.Extended;
						}
                        break;
                    }
                case 1:
                    {
                        inEdit = false;
                        editFilled = false;
                        searched = false;

                        dm.SetSearching(true);
                        TabChangeFormStateChange(false, false);
                        CustomerListBox.SelectionMode = SelectionMode.Extended;
                        break;
                    }
                case 2:
                    {
                        inEdit = true;
                        searched = false;

                        if (searchTextBox.Text.Length == 0)
                        {
                            dm.SetSearching(false);
                            ClearSearch();
                        }
                        UpdateEditField();
                        TabChangeFormStateChange(false, false);
                        CustomerListBox.SelectionMode = SelectionMode.Single;
                        break;
                    }
            }
        }

        
       
        private bool inEdit = false;
        private bool editFilled = false;
        private void UpdateEditField()
        {
            if (inEdit && CustomerListBox.SelectedItem != null && !editFilled)
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
			imagePathEditTextbox.Text = c.ImageFolder;
            editFilled = true;
        }

        private void UpdateEditOnClick(object sender, RoutedEventArgs e)
        {
			if (CustomerListBox.SelectedIndex >= dm.GetCustomerListCount()|| CustomerListBox.SelectedIndex == -1)
				return;

            Customer c = dm.Customers[CustomerListBox.SelectedIndex];
            int index = dm.GetAllCustomers().IndexOf(c);
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
			c.ImageFolder = imagePathEditTextbox.Text;
            dm.UpdateEditedItemInSearch(c, index, CustomerListBox.SelectedIndex);
            CustomerListBox.Items.Refresh();
            MessageBox.Show("Changes applied to ID: " + c.Id);
            editFilled = false;
        }

        private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            editFilled = false;
            UpdateEditField();
			if (CustomerListBox.SelectedIndex >= dm.GetCustomerListCount() || CustomerListBox.SelectedIndex == -1)
				return;
			FindImagesInFolder(dm.Customers[CustomerListBox.SelectedIndex].ImageFolder);
        }

        #endregion

        private void SortOptionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dm.sortManager.descending = sortOptionsComboBox.SelectedIndex == 1 ? true : false;
            dm.sortManager.SetSortedFalse();
        }

        private void SortListOnClick(object sender, RoutedEventArgs e)
        {
            dm.Sort();
        }


		private List<ImageItem> imagePaths = new List<ImageItem>();

		private void FindImagesInFolder(string folderPath)
		{
			imagePaths.Clear();
			imageBoxListView.Items.Refresh();
			if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
				return;

			foreach (string file in Directory.EnumerateFiles(folderPath, "*.jpg", SearchOption.AllDirectories)
				.Where(s => s.ToLower().EndsWith(".jpg") || s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".jpeg")))
			{
				imagePaths.Add(new ImageItem(file));
			}

			foreach(ImageItem s in imagePaths)
			{
				Console.WriteLine(s.path);
			}
			imageBoxListView.ItemsSource = imagePaths;
			imageBoxListView.Items.Refresh();
		}

		private void ClearImageListView()
		{
			imageBoxListView.ItemsSource = null;
			imageBoxListView.Items.Clear();
		}

    }

	public class ImageItem
	{
		public string path { get; set; }
		public ImageItem(string p)
		{
			path = p;
		}
	}
}
