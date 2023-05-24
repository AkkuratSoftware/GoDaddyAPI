// See https://aka.ms/new-console-template for more information
using IPUpdater;
using System.Net.NetworkInformation;

Console.WriteLine("Hello, World!");


//var ipdata = await MyIP.Load("http://ipinfo.io/json");

var keys = await Keys.Load();

Task.Run(() => RunCheck(keys));

Console.ReadKey();

async Task RunCheck(Keys keysConfig)
{


    while (true)
    {
        var dnsRecord = await ExistingDNS.Load(keysConfig);
        var ipdata = await MyIP.Load(keysConfig.IpApi);
        if (ipdata.Ip.Trim() != dnsRecord.Data.Trim())
        {
            Console.WriteLine("Records differ, Updating DNS");
            var testUpdate = await ExistingDNS.UpdateDNS(keysConfig, ipdata.Ip.Trim());
        }
        else
        {
            Console.WriteLine("Records are the same");
        }
        Thread.Sleep(300000);
    }
}