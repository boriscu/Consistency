using System;
using System.Runtime.Serialization;

namespace Sensors.Contracts
{
    [DataContract]
    public class SensorReadingDto
    {
        [DataMember(Order = 1, IsRequired = true)]
        public SensorId SensorId { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public double ValueCelsius { get; set; }

        [DataMember(Order = 3, IsRequired = false)]
        public DateTime? ClientTimestampUtc { get; set; }

        [DataMember(Order = 4, IsRequired = false)]
        public DateTime? ServerTimestampUtc { get; set; }

        [DataMember(Order = 5, IsRequired = false)]
        public string Source { get; set; }
    }
}
