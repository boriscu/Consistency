using System;
using Sensors.ServiceHost.Services;

namespace Sensors.ServiceHost.Hosting
{
    internal class Program
    {
        private static void Main()
        {
            // NOTE: We intentionally use the parameterless constructor here.
            // The base address is already specified in App.config under:
            //   <system.serviceModel><services><service><host><baseAddresses>...
            // If we also pass a baseAddress in code, WCF sees two net.tcp base addresses
            // and throws: "This collection already contains an address with scheme net.tcp…"
            using (var host = new System.ServiceModel.ServiceHost(typeof(SensorService)))
            {
                try
                {
                    host.Open();
                    Console.Title = "Sensors.ServiceHost";
                    Console.WriteLine("[ServiceHost] WCF service running at net.tcp://localhost:9001/SensorService");
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
                    if (host.State == System.ServiceModel.CommunicationState.Opened)
                        host.Close();
                    else
                        host.Abort();
                }
            }
        }
    }
}
