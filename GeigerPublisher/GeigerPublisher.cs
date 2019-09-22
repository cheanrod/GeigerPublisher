using System;
using System.Text;
using System.Threading.Tasks;
using GeigerPublisher.Values;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using shortid;

namespace GeigerPublisher
{

    interface IGeigerPublisher
    {
        Task publishReading(string reading);
        void Disconnect();
    }

    class MQTTPublisher : IGeigerPublisher
    {
        static MqttClient _client;
        const string MqttTopic = "geigercounter/values";
        const int PublishIntervall = 60;
        DateTime _lastPublish;

        public MQTTPublisher(string server)
        {
            _lastPublish = DateTime.MinValue;
            var clientId = $"geigercounter-{ ShortId.Generate(true, false).ToLower() } ";
            Console.WriteLine($"Connection to MQTT broker: { server }:1883 as clientID: { clientId }");
            _client = new MqttClient(server);
            _client.Connect(clientId);
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            _client.Disconnect();
        }

        public Task publishReading(string reading)
        {
            if (_lastPublish.AddSeconds(PublishIntervall) > DateTime.Now)
            {
                return Task.CompletedTask;
            }

            Values.GeigerValues geigerValues;
            try
            {
                geigerValues = GeigerValuesConverter.ConvertFromReading(reading);
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Cannot convert reading: { e }");
                return Task.CompletedTask;
            }
            _lastPublish = DateTime.Now;
            var json = GeigerValuesConverter.ConvertToJson(geigerValues);
            return Task.Run(() => { _client.Publish(MqttTopic, Encoding.UTF8.GetBytes(json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false); });
        }
    }

    class ConsolePublisher : IGeigerPublisher
    {
        public void Disconnect()
        {
            Console.WriteLine("Disconnecting...");
        }

        public Task publishReading(string reading)
        {
            return Task.Run(() => { Console.WriteLine(reading); });
        }
    }
}