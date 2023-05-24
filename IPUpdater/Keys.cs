using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IPUpdater
{
    internal class Keys
    {
        public string IpApi { get; set; }
        public string GetDnsApi { get; set; }
        public string Domain { get; set; }
        public string TestKey { get; set; }
        public string TestSecret { get; set; }
        public string ShopperId { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }

        public static async Task<Keys> Load()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string pathName = Path.Combine(workingDirectory, "Keys.json");
            var s = await File.ReadAllTextAsync(pathName);
            var sr = new StringReader(s);
            using (var jtr = new JsonTextReader(sr))
            {
                var obj = new JsonSerializer().Deserialize<Keys>(jtr);
                return obj;
            }
        }

        public void Save()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string pathName = Path.Combine(workingDirectory, "Keys.json");

           var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(pathName, json);

        }
    }
}
