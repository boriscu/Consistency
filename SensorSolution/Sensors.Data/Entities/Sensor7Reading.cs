using System;

namespace Sensors.Data.Entities
{
    public class Sensor7Reading
    {
        public int Id { get; set; }
        public DateTime TimestampUtc { get; set; } 
        public double ValueCelsius { get; set; }   
        public string Source { get; set; }         
    }
}
