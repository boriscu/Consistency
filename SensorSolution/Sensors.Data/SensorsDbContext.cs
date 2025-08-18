using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Sensors.Data.Entities;

namespace Sensors.Data
{
    public class SensorsDbContext : DbContext
    {
        public SensorsDbContext() : base("name=SensorsDb")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<Sensor1Reading> Sensor1Readings { get; set; }
        public DbSet<Sensor2Reading> Sensor2Readings { get; set; }
        public DbSet<Sensor3Reading> Sensor3Readings { get; set; }
        public DbSet<Sensor4Reading> Sensor4Readings { get; set; }
        public DbSet<Sensor5Reading> Sensor5Readings { get; set; }
        public DbSet<Sensor6Reading> Sensor6Readings { get; set; }
        public DbSet<Sensor7Reading> Sensor7Readings { get; set; }
        public DbSet<Sensor8Reading> Sensor8Readings { get; set; }
        public DbSet<Sensor9Reading> Sensor9Readings { get; set; }
        public DbSet<Sensor10Reading> Sensor10Readings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


            modelBuilder.Entity<Sensor1Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor2Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor3Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor4Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor5Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor6Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor7Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor8Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor9Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Sensor10Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);

            base.OnModelCreating(modelBuilder);
        }
    }
}
