using System;
using System.Collections.Generic;
using System.Configuration; // <-- needed for ConfigurationManager
using System.Linq;
using Sensors.Data;
using Sensors.Data.Entities;

namespace Sensors.ServiceHost.Jobs
{
    public static class ReplicateReconciliationJob
    {
        public static void RunOnce()
        {
            double tolerance = ReadDouble("Tolerance", 5.0);
            bool alignToWholeMinute = ReadBool("AlignToWholeMinute", true);

            using (var db = new SensorsDbContext())
            {
                var latest = new List<(DateTime ts, double value, int sensorIndex)>();

                var r1 = db.Sensor1Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r2 = db.Sensor2Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r3 = db.Sensor3Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r4 = db.Sensor4Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r5 = db.Sensor5Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r6 = db.Sensor6Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r7 = db.Sensor7Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r8 = db.Sensor8Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r9 = db.Sensor9Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();
                var r10 = db.Sensor10Readings.Where(x => x.Source == "Raw").OrderByDescending(x => x.TimestampUtc).FirstOrDefault();

                AddIfNotNull(latest, r1, 1);
                AddIfNotNull(latest, r2, 2);
                AddIfNotNull(latest, r3, 3);
                AddIfNotNull(latest, r4, 4);
                AddIfNotNull(latest, r5, 5);
                AddIfNotNull(latest, r6, 6);
                AddIfNotNull(latest, r7, 7);
                AddIfNotNull(latest, r8, 8);
                AddIfNotNull(latest, r9, 9);
                AddIfNotNull(latest, r10, 10);

                if (latest.Count == 0)
                    return; 

                var avg = latest.Average(x => x.value);

                var inRange = latest.Where(x => Math.Abs(x.value - avg) <= tolerance).ToList();

                (DateTime ts, double value) chosen;
                if (inRange.Any())
                {
                    var pick = inRange.OrderByDescending(x => x.ts).First();
                    chosen = (pick.ts, pick.value);
                }
                else
                {
                    var pick = latest.OrderByDescending(x => x.ts).First();
                    chosen = (pick.ts, pick.value);
                }

                var insertTs = alignToWholeMinute
                    ? AlignToMinute(DateTime.UtcNow)
                    : DateTime.UtcNow;

                var v = chosen.value;

                db.Sensor1Readings.Add(new Sensor1Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor2Readings.Add(new Sensor2Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor3Readings.Add(new Sensor3Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor4Readings.Add(new Sensor4Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor5Readings.Add(new Sensor5Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor6Readings.Add(new Sensor6Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor7Readings.Add(new Sensor7Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor8Readings.Add(new Sensor8Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor9Readings.Add(new Sensor9Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });
                db.Sensor10Readings.Add(new Sensor10Reading { TimestampUtc = insertTs, ValueCelsius = v, Source = "Reconciled" });

                db.SaveChanges();

                Console.WriteLine($"[Reconcile] avg={avg:F2}, chosen={v:F2}, ts={insertTs:HH:mm:ss} UTC");
            }
        }

        private static void AddIfNotNull(List<(DateTime ts, double value, int sensorIndex)> list, dynamic e, int idx)
        {
            if (e != null)
                list.Add((e.TimestampUtc, e.ValueCelsius, idx));
        }

        private static DateTime AlignToMinute(DateTime utcNow)
            => new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, 0, DateTimeKind.Utc);

        private static double ReadDouble(string key, double fallback)
            => double.TryParse(ConfigurationManager.AppSettings[key], out var v) ? v : fallback;

        private static bool ReadBool(string key, bool fallback)
            => bool.TryParse(ConfigurationManager.AppSettings[key], out var v) ? v : fallback;
    }
}
