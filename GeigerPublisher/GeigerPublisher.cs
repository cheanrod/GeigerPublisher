using System;
using System.Text;
using System.Threading.Tasks;
using GeigerPublisher.Values;
using shortid;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client;
using System.Threading;

namespace GeigerPublisher
{

    interface IGeigerPublisher
    {
        void publishReading(string reading);
        Task Connect();
        Task Disconnect();
    }

    class MQTTPublisher : IGeigerPublisher
    {
        private IMqttClient _client;
        private string _server;
        private const string MqttTopic = "geigercounter/values";
        private const int PublishIntervall = 60;
        private DateTime _lastPublish;

        public MQTTPublisher(string server)
        {
            _server = server;
            _lastPublish = DateTime.MinValue;
        }

        public async Task Connect()
        {
            var clientId = $"geigercounter-{ ShortId.Generate(true, false).ToLower() } ";
            Console.WriteLine($"Connection to MQTT broker: { _server }:1883 as clientID: { clientId }");

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(_server, 1883)
            .Build();
            setDisconnectHandler(options);
            await _client.ConnectAsync(options, CancellationToken.None);
        }

        private void setDisconnectHandler(IMqttClientOptions options)
        {
            _client.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await _client.ConnectAsync(options, CancellationToken.None);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });
        }

        public async Task Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            await _client.DisconnectAsync();
        }

        public async void publishReading(string reading)
        {
            if (_lastPublish.AddSeconds(PublishIntervall) > DateTime.Now)
            {
                return;
            }

            Values.GeigerValues geigerValues;
            try
            {
                geigerValues = GeigerValuesConverter.ConvertFromReading(reading);
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Cannot convert reading: { e }");
                return;
            }
            _lastPublish = DateTime.Now;
            var json = GeigerValuesConverter.ConvertToJson(geigerValues);
            
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopic)
            .WithPayload(json)
            .Build();
            await _client.PublishAsync(message);
        }
    }

    class ConsolePublisher : IGeigerPublisher
    {
        public Task Connect()
        {
            Console.WriteLine("Connecting...");
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            return Task.CompletedTask;
        }

        public void publishReading(string reading)
        {
            Console.WriteLine(reading);
            return;
        }
    }
}
