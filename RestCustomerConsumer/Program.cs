using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestCustomerConsumer
{
    class Program
    {
        static HttpClient client = new HttpClient();
        //the uri is pointing to our local service we have created
        private static string uri = "https://localhost:44305/customer";
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
        
        static async void RunAsync()
        {
            //gets all customers and displays them
            IList<Customer> getAllList = await GetAllCustomersAsync();
            ShowObjectData(getAllList);
            Console.WriteLine("------ Get Customer By Id 3 -------");
            //Gets Customer by Id
            var getCustomerById = await GetCustomersById("3");
            ShowSingleObjectData(getCustomerById);
            //Creates New Customer
            Console.WriteLine("---- Rick James is being Added ----");
            Customer newCustomer = new Customer() { Id = 13, FirstName = "Rick", LastName = "James", Year = 1969};
            //creates local variable new resource which contains the new customer created above
            var newResource = await AddNewCustomer(newCustomer);
            //prints out the single new data entry
            ShowSingleObjectData(newResource);
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


        static async Task<Customer> GetCustomersById(string id)
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
                throw new Exception("Customer Id was not found");
            }
        }


        static async Task<Customer> AddNewCustomer(Customer newCust)
        {
            var jsonContent = JsonConvert.SerializeObject(newCust);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(uri, content);
            if (response.StatusCode != HttpStatusCode.Conflict)
            {
                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var newlyCreatedCustomer = JsonConvert.DeserializeObject<Customer>(jsonString);
                return newlyCreatedCustomer;
            }
            else
            {
                throw new Exception("Customer already exists!");
            }
        }


        static async Task<Customer> UpdateResource(Customer cust, string id)
        {
            var jsonContent = JsonConvert.SerializeObject(cust);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(uri + "/" + id, content);
            if (response.StatusCode != HttpStatusCode.Conflict)
            {
                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var newlyCreatedCustomer = JsonConvert.DeserializeObject<Customer>(jsonString);
                return newlyCreatedCustomer;
            }
            else
            {
                throw new Exception("Customer already exists!");
            }
        }


        static async void DeleteToDoItemAsync(string id)
        {
            string uriId = uri + "/" + id;
            HttpResponseMessage response = await client.DeleteAsync(uriId);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("Customer not found with that Id. Check Id was entered correctly and try again!");
            }
            response.EnsureSuccessStatusCode();
        }



        //takes the IList created above and goes through and prints out each Customers Data
        static void ShowObjectData(IList<Customer> cList)
        {
            foreach (var c in cList)
            {
                Console.WriteLine("Customer ID: " + c.Id + ", Name: " + c.FirstName + " " + c.LastName);
            }
        }
        static void ShowSingleObjectData(Customer cust)
        {
            Console.WriteLine("Customer ID: " + cust.Id + ", Name: " + cust.FirstName + " " + cust.LastName);
        }

    }
}
