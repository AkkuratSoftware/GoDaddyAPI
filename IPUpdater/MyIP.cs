using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace IPUpdater
{
    internal class MyIP
    {
        public string Ip { get; set; }
        public string Hostname { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Loc { get; set; }
        public string Org { get; set; }
        public string Postal { get; set; }
        public string Timezone { get; set; }
        public string Readme { get; set; }

        public static async Task<MyIP> Load(string url)
        {
            HttpClient client = new HttpClient(); 

            using (var req = await client.GetAsync(url))
            {
                req.EnsureSuccessStatusCode();
                using (var s = await req.Content.ReadAsStreamAsync())
                using (var sr = new StreamReader(s))
                using (var jtr = new JsonTextReader(sr))
                {
                    var obj = new JsonSerializer().Deserialize<MyIP>(jtr);
                    return obj;
                }

            }

        }


    }
}
