using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace GeigerPublisher
{
    interface IGeigerReader
    {
        Task StartRead(Action<string> returnLine);
    }

    class SerialReader : IGeigerReader
    {
        private SerialPort _serialPort;
        private CancellationToken _token;

        public SerialReader(string serialPort, CancellationToken token)
        {
            _token = token;
            _serialPort = new SerialPort();
            _serialPort.PortName = serialPort;
            _serialPort.BaudRate = 9600;
        }

        public async Task StartRead(Action<string> returnLine)
        {
            _serialPort.Open();
            using (StreamReader reader = new StreamReader(_serialPort.BaseStream, System.Text.Encoding.ASCII))
            {
                while (!_token.IsCancellationRequested)
                {
                    String result;
                    result = await reader.ReadLineAsync();
                    Console.WriteLine($"Line contains: { result }");
                    returnLine(result);
                }
            }
            Console.WriteLine("Closing serial port...");
            _serialPort.Close();
        }
    }

    class InfinityReader : IGeigerReader
    {
        private CancellationToken _token;

        private const string Value = "CPS, 2, CPM, 42, uSv/hr, 0.03, SLOW";

        public InfinityReader(CancellationToken token)
        {
            _token = token;
        }

        public async Task StartRead(Action<string> returnLine)
        {
            while (!_token.IsCancellationRequested)
            {
                await Task.Delay(5000);
                returnLine(Value);
            }
        }
    }
}