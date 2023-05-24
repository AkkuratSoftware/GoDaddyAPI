using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater
{
    internal class DNSUpdate
    {
        public string Data { get; set; }
        public int Port { get; set; } = 1;
        public int Priority { get; set; } = 1;
        //public string Protocol { get; set; } ="string";
        //public string Service { get; set; } = "string";
        public int Ttl { get; set; } = 3600;
        public int Weight { get; set; } = 1;

        public DNSUpdate(string ip)
        {
            Data = ip;
        }
    }
}
