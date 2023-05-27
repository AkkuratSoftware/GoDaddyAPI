// See https://aka.ms/new-console-template for more information
using IPUpdater;
using System.Net.NetworkInformation;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using HiveMQtt.MQTT5.Types;
using HiveMQtt.Client.Results;

var keys = await Keys.Load();
var  mqClient = GetConfiguredMQClient();

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
            var updatemsg = String.Format("DNS Updated to {0}", ipdata.Ip);
            var result = await mqClient.PublishAsync(keysConfig.Topic, updatemsg , QualityOfService.ExactlyOnceDelivery).ConfigureAwait(false);

        }
        else
        {
            if (keysConfig.SendNotUpdated)
            {
                var connectResult = await mqClient.ConnectAsync().ConfigureAwait(false);
                var result = await mqClient.PublishAsync(keysConfig.SkippedTopic, "DNS Not Updated", QualityOfService.ExactlyOnceDelivery).ConfigureAwait(false);
            }
                
            Console.WriteLine(string.Format("Records are the same Current IP Adress is {0}", ipdata.Ip));
        }
        Thread.Sleep(60000);
    }
}

HiveMQClient GetConfiguredMQClient()
{
    var options = GetMQOptions();
    var client = new HiveMQClient(options);

    return client;
}

HiveMQClientOptions GetMQOptions()
{
    var options = new HiveMQClientOptions
    {
        Host = keys.ClusterUrl,
        Port = keys.MQPort,
        UseTLS = true,
        UserName = keys.MQUser,
        Password = keys.MQSecret,
    };

    return options;
}