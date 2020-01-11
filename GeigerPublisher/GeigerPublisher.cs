using System;
using System.Linq;
using System.Threading.Tasks;
using GeigerPublisher.Values;
using shortid;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client;
using System.Threading;
using System.Collections.Generic;

namespace GeigerPublisher
{

    interface IGeigerPublisher
    {
        Task PublishReading(string reading);
        Task PublishConnectedMessage();
        Task Connect();
        Task Disconnect();
    }

    class MQTTPublisher : IGeigerPublisher
    {
        private IMqttClient _client;
        private string _server;
        private const string MqttTopic = "geigercounter/status";
        private const int PublishIntervall = 60;
        private readonly IList<GeigerValues> _geigerValues = new List<GeigerValues>();
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
            .WithWillMessage(new MqttApplicationMessageBuilder()
                .WithTopic("geigercounter/connected")
                .WithPayload("0")
                .WithRetainFlag(true)
                .Build())
            .Build();
            
            setDisconnectHandler(options);
            
            await _client.ConnectAsync(options, CancellationToken.None);
            await _client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic("geigercounter/connected")
                .WithPayload("1")
                .WithRetainFlag(true)
                .Build());
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
            await _client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic("geigercounter/connected")
                .WithPayload("0")
                .WithRetainFlag(true)
                .Build());
            await _client.DisconnectAsync();
        }

        public async Task PublishReading(string reading)
        {
            Values.GeigerValues geigerValue;
            try
            {
                geigerValue = GeigerValuesConverter.ConvertFromReading(reading);
                _geigerValues.Add(geigerValue);
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Cannot convert reading: { e }");
                return;
            }
            
            if (_lastPublish.AddSeconds(PublishIntervall) <= DateTime.Now)
            {
                _lastPublish = DateTime.Now;
                var json = GeigerValuesConverter.ConvertToJson(_lastPublish, _geigerValues.Select(x => x.Radiation));

                var message = new MqttApplicationMessageBuilder()
                .WithTopic(MqttTopic)
                .WithPayload(json)
                .WithRetainFlag(true)
                .Build();
                await _client.PublishAsync(message);
                _geigerValues.Clear();
            }
        }

        public async Task PublishConnectedMessage()
        {
            await _client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic("geigercounter/connected")
                .WithPayload("2")
                .WithRetainFlag(true)
                .Build());
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

        public Task PublishConnectedMessage()
        {
            Console.WriteLine("Publisher connected.");
            return Task.CompletedTask;
        }

        public Task PublishReading(string reading)
        {
            Console.WriteLine(reading);
            return Task.CompletedTask;
        }
    }
}
