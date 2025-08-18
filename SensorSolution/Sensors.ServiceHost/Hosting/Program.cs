using System;
using System.ServiceModel;
using System.Threading;
using Sensors.ServiceHost.Jobs;
using Sensors.ServiceHost.Services;

namespace Sensors.ServiceHost.Hosting
{
    internal class Program
    {
        private static Timer _timer;

        private static void Main()
        {
            using (var host = new System.ServiceModel.ServiceHost(typeof(SensorService)))
            {
                try
                {
                    host.Open();
                    Console.Title = "Sensors.ServiceHost";
                    Console.WriteLine("[ServiceHost] WCF service running at net.tcp://localhost:9001/SensorService");

                    // Start reconciliation timer: first tick after 60s, then every 60s (read from config inside the job if you prefer)
                    _timer = new Timer(_ => SafeRun(), null, dueTime: TimeSpan.FromSeconds(60), period: TimeSpan.FromSeconds(60));
                    Console.WriteLine("[ServiceHost] Reconciliation timer started (every 60s).");
                    Console.WriteLine("[ServiceHost] Press ENTER to stop.");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to start WCF host: " + ex);
                    Console.ReadLine();
                }
                finally
                {
                    _timer?.Dispose();

                    if (host.State == CommunicationState.Opened)
                        host.Close();
                    else
                        host.Abort();
                }
            }
        }

        private static void SafeRun()
        {
            try
            {
                ReplicateReconciliationJob.RunOnce();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Reconcile] ERROR: " + ex.Message);
            }
        }
    }
}