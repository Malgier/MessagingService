using DomainModel;
using MessagingMicroService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessagingMicroService
{
    public class Client
    {
        public User GetUser(string url, string location)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

                HttpResponseMessage response = client.GetAsync(location).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<User>().Result;

                    response.Dispose();

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<User> GetUsers(string url, string location)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

                HttpResponseMessage response = client.GetAsync(location).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<List<User>>().Result;

                    response.Dispose();

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
