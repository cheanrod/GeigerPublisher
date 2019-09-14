using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace GeigerPublisher
{
    interface IGeigerReader
    {
        Task StartRead();
    }

    class SerialReader : IGeigerReader
    {
        static SerialPort _serialPort;
        static IGeigerPublisher _publisher;
        static CancellationToken _token;

        public SerialReader(string serialPort, IGeigerPublisher publisher, CancellationToken token)
        {
            _publisher = publisher;
            _token = token;
            _serialPort = new SerialPort();
            _serialPort.PortName = serialPort;
            _serialPort.BaudRate = 9600;
        }

        public async Task StartRead()
        {
            while (!_token.IsCancellationRequested)
            {
                _serialPort.Open();
                using (StreamReader reader = new StreamReader(_serialPort.BaseStream, System.Text.Encoding.ASCII))
                {
                    String result;
                    result = await reader.ReadLineAsync();
                    Console.WriteLine("Line contains: " + result);
                    await _publisher.publishReading(result);
                }
            }
            Console.WriteLine("Closing serial port...");
            _serialPort.Close();
        }
    }

    class InfinityReader : IGeigerReader
    {
        IGeigerPublisher _publisher;
        static CancellationToken _token;

        private const string Value = "CPS, 2, CPM, 42, uSv/hr, 0.03, SLOW";

        public InfinityReader(IGeigerPublisher publisher, CancellationToken token)
        {
            _publisher = publisher;
            _token = token;
        }

        public async Task StartRead()
        {
            while (!_token.IsCancellationRequested)
            {
                await Task.Delay(5000);
                await _publisher.publishReading(Value);
            }
        }
    }
}