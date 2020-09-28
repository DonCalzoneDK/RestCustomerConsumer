using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestCustomerConsumer
{
    class Program
    {
        static HttpClient client = new HttpClient();
        //the uri is pointing to our local service we have created
        private static string uri = "https://localhost:44346/customer";
        //in the main we execute the RunAsync method which utilizes two other methods
        //the first is an async method to compile all the customers into a list(GetAllCustomersAsync)
        //and the second ShowObjectData prints our the customers Id + first name and last name
        //and then in the main it will read out this data
        static void Main(string[] args)
        {
            try
            {
                RunAsync();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
            }
        }
        //gets all customers and displays them
        static async void RunAsync()
        {
            IList<Customer> getAllList = await GetAllCustomersAsync();
            ShowObjectData(getAllList);
        }

        //gets all customers from service we created
        public static async Task<IList<Customer>> GetAllCustomersAsync()
        {
            //using will close this connection automatically
            //when the service is ended(after returning cList)
            using (HttpClient client = new HttpClient())
            {
                //creating a local variable containing our HTTPClient a get request and our uri route
                string content = await client.GetStringAsync(uri);
                //creates a IList of Customers called cList then deserializes it from Json to .NET type
                //of class Customer and using the local variable (content) created above
                IList<Customer> cList = JsonConvert.DeserializeObject<IList<Customer>>(content);
                //returns cList
                return cList;
            }
        }


        static async Task<Customer> GetAllCustomersById(string id)
        {
            string uriId = uri + "/" + id;
            HttpResponseMessage response = await client.GetAsync(uriId);
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
                string jsonContent = await response.Content.ReadAsStringAsync();
                var singleObj = JsonConvert.DeserializeObject<Customer>(jsonContent);
                return singleObj;
            }
            else
            {
                throw new Exception("Customer Id is not found");
            }
        }



        //takes the IList created above and goes through and prints out each Customers Data
        static void ShowObjectData(IList<Customer> cList)
        {
            foreach (var c in cList)
            {
                Console.WriteLine("Customer ID: " + c.Id + ", Name: " + c.FirstName + " " + c.LastName);
            }
        }

    }
}
