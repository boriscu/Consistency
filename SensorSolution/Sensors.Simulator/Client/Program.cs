using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sensors.Contracts;
using Sensors.Simulator.Client;

namespace Sensors.Simulator
{
    internal class Program
    {
        private static async Task<int> Main()
        {
            Console.Title = "Sensors.Simulator";
            Console.WriteLine("Starting 10 sensor workers (1–10s intervals). Press Ctrl+C to stop.");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var tasks = new List<Task>();
            int seedBase = Environment.TickCount;

            // Start S1..S10
            for (int i = 1; i <= 10; i++)
            {
                var worker = new SensorWorker((SensorId)i, seedBase + i);
                tasks.Add(worker.RunAsync(cts.Token));
            }

            await Task.WhenAll(tasks);

            WcfClientFactory.Close();

            Console.WriteLine("Simulator stopped.");
            return 0;
        }
    }
}
