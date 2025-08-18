using System;
using System.ServiceModel;
using Sensors.Contracts;

namespace Sensors.Simulator.Client
{

    public static class WcfClientFactory
    {
        private static readonly object _lock = new object();
        private static ChannelFactory<ISensorService> _factory;
        private static ISensorService _channel;

        public static ISensorService GetChannel()
        {
            lock (_lock)
            {
                if (_factory == null)
                {
                    var binding = new NetTcpBinding(); 
                    var address = new EndpointAddress("net.tcp://localhost:9001/SensorService");
                    _factory = new ChannelFactory<ISensorService>(binding, address);
                }

                if (_channel == null || ((IClientChannel)_channel).State != CommunicationState.Opened)
                {
                    _channel = _factory.CreateChannel();
                    ((IClientChannel)_channel).Open();
                }

                return _channel;
            }
        }

        public static void Close()
        {
            lock (_lock)
            {
                try
                {
                    if (_channel != null)
                    {
                        var cc = (IClientChannel)_channel;
                        if (cc.State == CommunicationState.Opened) cc.Close();
                        else cc.Abort();
                        _channel = null;
                    }

                    if (_factory != null)
                    {
                        if (_factory.State == CommunicationState.Opened) _factory.Close();
                        else _factory.Abort();
                        _factory = null;
                    }
                }
                catch
                {
                    _channel = null;
                    _factory = null;
                }
            }
        }
    }
}
