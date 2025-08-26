using System;
using System.Threading;
using System.Threading.Tasks;
using Sensors.Contracts;

namespace Sensors.Simulator.Client
{
    public class SensorWorker
    {
        private readonly SensorId _sensorId;
        private readonly Random _rng;

        public SensorWorker(SensorId sensorId, int seed)
        {
            _sensorId = sensorId;
            _rng = new Random(seed);
        }

        public async Task RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var delayMs = _rng.Next(1, 11) * 1000;
                    await Task.Delay(delayMs, ct);

                    var value = 18.0 + _rng.NextDouble() * 12.0;

                    var channel = WcfClientFactory.GetChannel();
                    channel.SubmitReading(new SensorReadingDto
                    {
                        SensorId = _sensorId,
                        ValueCelsius = value,
                        ClientTimestampUtc = DateTime.UtcNow,
                        Source = "Raw"
                    });

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sent {_sensorId}: {value:F2} °C");
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{_sensorId}] send error: {ex.Message}");
                    await Task.Delay(1000, ct);
                }
            }
        }
    }
}
