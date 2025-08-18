using System;
using System.Linq;
using Sensors.Contracts;
using Sensors.Data;
using Sensors.Data.Entities;

namespace Sensors.ServiceHost.Services
{
    public class SensorService : ISensorService
    {
        public void SubmitReading(SensorReadingDto reading)
        {
            if (reading == null) return;

            var serverNow = DateTime.UtcNow;
            using (var db = new SensorsDbContext())
            {
                switch (reading.SensorId)
                {
                    case SensorId.S1:
                        db.Sensor1Readings.Add(new Sensor1Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S2:
                        db.Sensor2Readings.Add(new Sensor2Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S3:
                        db.Sensor3Readings.Add(new Sensor3Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S4:
                        db.Sensor4Readings.Add(new Sensor4Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S5:
                        db.Sensor5Readings.Add(new Sensor5Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S6:
                        db.Sensor6Readings.Add(new Sensor6Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S7:
                        db.Sensor7Readings.Add(new Sensor7Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S8:
                        db.Sensor8Readings.Add(new Sensor8Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S9:
                        db.Sensor9Readings.Add(new Sensor9Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    case SensorId.S10:
                        db.Sensor10Readings.Add(new Sensor10Reading { TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw" }); break;
                    default:
                        return;
                }
                db.SaveChanges();
            }
        }

        public SensorReadingDto GetLatestReading(int sensorId)
        {
            using (var db = new SensorsDbContext())
            {
                switch ((SensorId)sensorId)
                {
                    case SensorId.S1:
                        return Project(db.Sensor1Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S1);
                    case SensorId.S2:
                        return Project(db.Sensor2Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S2);
                    case SensorId.S3:
                        return Project(db.Sensor3Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S3);
                    case SensorId.S4:
                        return Project(db.Sensor4Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S4);
                    case SensorId.S5:
                        return Project(db.Sensor5Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S5);
                    case SensorId.S6:
                        return Project(db.Sensor6Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S6);
                    case SensorId.S7:
                        return Project(db.Sensor7Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S7);
                    case SensorId.S8:
                        return Project(db.Sensor8Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S8);
                    case SensorId.S9:
                        return Project(db.Sensor9Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S9);
                    case SensorId.S10:
                        return Project(db.Sensor10Readings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.S10);
                    default:
                        return null;
                }
            }
        }

        public string Ping() => "OK";

        private static SensorReadingDto Project(object entity, SensorId id)
        {
            if (entity == null) return null;

            switch (entity)
            {
                case Sensor1Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor2Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor3Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor4Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor5Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor6Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor7Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor8Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor9Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                case Sensor10Reading e: return new SensorReadingDto { SensorId = id, ValueCelsius = e.ValueCelsius, ServerTimestampUtc = e.TimestampUtc, Source = e.Source };
                default: return null;
            }
        }
    }
}
