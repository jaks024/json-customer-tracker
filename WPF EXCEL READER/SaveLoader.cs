using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
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
            File.WriteAllText(path, JsonConvert.SerializeObject(dm.customers));           
        }
    }
}
