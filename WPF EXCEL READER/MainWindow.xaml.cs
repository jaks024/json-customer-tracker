﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        //load json save file into DataManager
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                pathTextBlock.Text = openFileDialog.FileName;
                DataManager.AddCustomer(SaveLoader.ReadSave(openFileDialog.FileName));
                UpdateItemCountText();
                CustomerListBox.ItemsSource = DataManager.GetAllCustomers();
            }
        }

        //clear path and all item in DataManager
        private void BtnClearFile_Click(object sender, RoutedEventArgs e)
        {
            pathTextBlock.Text = "";
            DataManager.ClearDataPresent();
            UpdateItemCountText();
        }


        private void UpdateItemCountText()
        {
            itemCountTextBox.Text = "Item Count: " + DataManager.GetListCount();
        }


        private static readonly Regex numberRegex = new Regex("[^0-9]");
        private void PhoneNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = numberRegex.IsMatch(e.Text);
        }

        private void AddNewCustomerOnClick(object sender, RoutedEventArgs e)
        {
            Customer c = new Customer();
            c.Name = nameTextBox.Text;
            c.PhoneNumber = phoneNumberTextBox.Text;
            c.Address = string.Format("{0}, {1}, {2}", streetTextBox.Text, cityTextBox.Text, postalTextBox.Text);

            DataManager.AddCustomer(c);
            UpdateItemCountText();
        }
    }
}