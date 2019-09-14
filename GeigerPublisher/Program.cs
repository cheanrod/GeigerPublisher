using System;
using Unity;
using System.Threading;
using System.Threading.Tasks;

namespace GeigerPublisher
{
    class Program
    {
        static UnityContainer _container;
        static IGeigerReader _reader;
        static IGeigerPublisher _publisher;
        private static readonly CancellationTokenSource _source = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            if (args.Length < 2 || args.Length > 2)
            {
                Console.WriteLine("Usage: GeigerPublisher <serial port> <broker hostname>");
                Console.WriteLine("Exiting...");
                Environment.Exit(1);
            }
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                _source.Cancel();
                eventArgs.Cancel = true;
            };

            _container = new UnityContainer();
            _container.RegisterInstance<IGeigerPublisher>(new MQTTPublisher(args[1]));
            _publisher = _container.Resolve<IGeigerPublisher>();

            _container.RegisterInstance<IGeigerReader>(new SerialReader(args[0], _publisher, _source.Token));
            _reader = _container.Resolve<IGeigerReader>();

            await _reader.StartRead();

            _publisher?.Disconnect();
            Console.WriteLine("Geigerpublisher exiting...");
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
