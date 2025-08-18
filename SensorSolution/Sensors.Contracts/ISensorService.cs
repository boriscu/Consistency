using System.ServiceModel;

namespace Sensors.Contracts
{
    [ServiceContract]
    public interface ISensorService
    {

        [OperationContract(IsOneWay = true)]
        void SubmitReading(SensorReadingDto reading);

        [OperationContract]
        SensorReadingDto GetLatestReading(int sensorId);

        [OperationContract]
        string Ping();
    }
}
