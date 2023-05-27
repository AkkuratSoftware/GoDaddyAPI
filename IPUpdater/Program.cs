// See https://aka.ms/new-console-template for more information
using IPUpdater;
using System.Net.NetworkInformation;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using HiveMQtt.MQTT5.Types;

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
        Thread.Sleep(60000);
    }
}