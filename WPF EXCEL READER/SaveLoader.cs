using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using Microsoft.Win32;
namespace WPF_EXCEL_READER
{
    class SaveLoader
    {

        public static List<Customer> ReadSave(string _path)
        {
            using (StreamReader reader = File.OpenText(_path))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(reader.ReadToEnd());
                return customers;
            }
            
        }

        public static void WriteToNewFile(Customer _customers, string path)
        {
            using (StreamWriter file = File.CreateText(@"C:\Users\Jackson\Desktop\save.json"))
            {
                JsonSerializer serializer = new JsonSerializer();

                //deseralize directly to file
                serializer.Serialize(file, _customers);

                /*{
                 *  "Id": 0,
                 *  "Name": "John",
                 *  "PhoneNumber": "647-675-7591",
                 *  "Address": "123 Feltham Rd, Markham, ON, CA"
                 *}
                 */
            }
        }

        public static void WriteToExistingFile(string path, DataManager dm)
        {
            if (path.Equals("") || path == null)
            {
                if (dm.GetCustomerListCount() == 0)
                {
                    MessageBox.Show("Invalid Operation, There are no entries");
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(dm.GetAllCustomers(), Formatting.Indented));
                    path = saveFileDialog.FileName;
                } else
                {
                    return;
                }
                
            }
            else
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(dm.GetAllCustomers(), Formatting.Indented));
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e.Message);
                }
            }
            MessageBox.Show(string.Format("Saved {0} entries to {1}", dm.GetCustomerListCount(), path));
        }
    }
}
