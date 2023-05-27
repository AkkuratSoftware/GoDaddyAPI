// See https://aka.ms/new-console-template for more information
// Program.cs essentially encapsulates the entire service.

using IPUpdater;
using System.Net.NetworkInformation;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using HiveMQtt.MQTT5.Types;
using HiveMQtt.Client.Results;

// This is a flag telling us the no dns update message has been sent or not
// It is reset after the DNS record is updated.
bool noUpdateSent = false;

//Housekeeping
var keys = await Keys.Load();
var  mqClient = GetConfiguredMQClient();

//At this point we have the configuration and the MQTT client connection 
//Send out the Service Start Message to the MQ
await SendServiceStart(keys);

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
            var connectResult = await mqClient.ConnectAsync().ConfigureAwait(false);
            Console.WriteLine("Records differ, Updating DNS");
            var testUpdate = await ExistingDNS.UpdateDNS(keysConfig, ipdata.Ip.Trim());
            var updatemsg = String.Format("DNS Updated to {0}", ipdata.Ip);
            var result = await mqClient.PublishAsync(keysConfig.Topic, updatemsg , QualityOfService.ExactlyOnceDelivery).ConfigureAwait(false);
            await mqClient.DisconnectAsync().ConfigureAwait(false);
            noUpdateSent = true;
        }
        else
        {
            if (keysConfig.SendNotUpdated && !noUpdateSent)
            {
                var connectResult = await mqClient.ConnectAsync().ConfigureAwait(false);
                var result = await mqClient.PublishAsync(keysConfig.SkippedTopic, "DNS Not Updated", QualityOfService.ExactlyOnceDelivery).ConfigureAwait(false);
                await mqClient.DisconnectAsync().ConfigureAwait(false);
                noUpdateSent = true;
            }
                
            Console.WriteLine(string.Format("Records are the same Current IP Adress is {0}", ipdata.Ip));
        }
        Thread.Sleep(60000);
    }
}

async Task SendServiceStart(Keys keysConfig)
{
    var serviceStartMsg = string.Format("IPUdater Service Started on {0}", System.Environment.MachineName);
    var connectResult = await mqClient.ConnectAsync().ConfigureAwait(false);
    var result = await mqClient.PublishAsync(keysConfig.ServiceStartTopic, serviceStartMsg, QualityOfService.ExactlyOnceDelivery).ConfigureAwait(false);
    await mqClient.DisconnectAsync().ConfigureAwait(false);
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