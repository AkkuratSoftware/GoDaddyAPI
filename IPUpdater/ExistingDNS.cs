using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater
{
    internal class ExistingDNS
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public int? Ttl { get; set; }
        public string Type { get; set; }

        public static async Task<ExistingDNS> Load(Keys configKeys)
        {

            HttpClient dnsClient = new HttpClient();

            //dnsClient.BaseAddress = new Uri(string.Format("{0}//{1}/A/@", configKeys.GetDnsApi,configKeys.Domain));
            dnsClient.DefaultRequestHeaders.Add("Authorization", string.Format("sso-key {0}:{1}", configKeys.Key, configKeys.Secret));
            dnsClient.DefaultRequestHeaders.Add("X-Shopper-Id", configKeys.ShopperId);
            dnsClient.DefaultRequestHeaders.Add("Accept", "application/json");



            using (var req = await dnsClient.GetAsync(string.Format(@"{0}/{1}/records/A/@", configKeys.GetDnsApi, configKeys.Domain)))
            {
                req.EnsureSuccessStatusCode();
                using (var s = await req.Content.ReadAsStreamAsync())
                using (var sr = new StreamReader(s))
                using (var jtr = new JsonTextReader(sr))
                {
                    var obj = new JsonSerializer().Deserialize<List<ExistingDNS>>(jtr);
                    return obj.FirstOrDefault();
                }

            }

        }

        public static async Task<string> UpdateDNS(Keys configKeys, string newIP)
        {
            var url = string.Format(@"{0}/{1}/records/A/@", configKeys.GetDnsApi, configKeys.Domain);
            var content = new StringContent(JsonConvert.SerializeObject(new[]
            {
                new {
                    ttl = 3600,
                    data = newIP
                }
            }), Encoding.UTF8, "application/json");

            HttpClient dnsClient = new HttpClient();

            dnsClient.DefaultRequestHeaders.Add("Authorization", string.Format("sso-key {0}:{1}", configKeys.Key, configKeys.Secret));
            dnsClient.DefaultRequestHeaders.Add("X-Shopper-Id", configKeys.ShopperId);
            dnsClient.DefaultRequestHeaders.Add("Accept", "application/json");
            dnsClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            using (var req = await dnsClient.PutAsync(url, content))
            {
                req.EnsureSuccessStatusCode();
                var s = await req.Content.ReadAsStringAsync();

                return s;

            }

        }

    }


}
