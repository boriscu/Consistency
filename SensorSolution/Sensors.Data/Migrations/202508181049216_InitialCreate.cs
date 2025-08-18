namespace Sensors.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sensor10Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor1Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor2Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor3Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor4Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor5Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor6Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor7Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor8Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensor9Reading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimestampUtc = c.DateTime(nullable: false),
                        ValueCelsius = c.Double(nullable: false),
                        Source = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sensor9Reading");
            DropTable("dbo.Sensor8Reading");
            DropTable("dbo.Sensor7Reading");
            DropTable("dbo.Sensor6Reading");
            DropTable("dbo.Sensor5Reading");
            DropTable("dbo.Sensor4Reading");
            DropTable("dbo.Sensor3Reading");
            DropTable("dbo.Sensor2Reading");
            DropTable("dbo.Sensor1Reading");
            DropTable("dbo.Sensor10Reading");
        }
    }
}
