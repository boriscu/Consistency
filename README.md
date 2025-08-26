# 1) Uvod

- **Cilj:** WCF sistem koji simulira **10 senzora temperature**; svaki 1–10 sekundi šalje nasumičnu temperaturu u SQL bazu (EF6).
- **Replikaciono poravnanje:** Svakih **60 sekundi** računa se „konsenzus“ i upisuje u **svih 10** tabela kao `Source = "Reconciled"`.
- **Pravilo konsenzusa:** Na svakih 60 s uzimamo pregled sistema, po jedno najnovije RAW očitavanje iz svake od 10 tabela. Iz tih 10 vrednosti izračunamo prosek. Konsenzus je ona vrednost iz tog pregleda koja je u opsegu ±Tolerance (podrazumevano 5 °C) oko proseka i pritom je vremenski najskorija. Ako nijedna od 10 najnovijih vrednosti ne upada u taj opseg, kao konsenzus se uzima vremenski najskorija vrednost iz pregleda (bez obzira na odstupanje). Ta vrednost se zatim upisuje u svih 10 tabela sa Source="Reconciled" (vreme poravnato na minut).
- **Tehnologije:** .NET Framework 4.8, WCF (net.tcp), Entity Framework 6.4.4, SQL LocalDB (MSSQLLocalDB).

# 2) Arhitektura

Simulator pokreće 10 zadataka. Svaki zadatak na 1–10 sekundi generiše temperaturu i poziva SubmitReading preko net.tcp. Poziv je jednosmeran pa Simulator ne čeka odgovor.

ServiceHost izlaže ISensorService na net.tcp://localhost:9001/SensorService. SubmitReading servis uzima vreme sa servera u UTC, bira tabelu na osnovu SensorId i upisuje red sa Source = Raw. Upis radi EF6 preko SensorsDbContext. Klijent nema pristup bazi.

Svakih 60 sekundi tajmer u ServiceHost pokreće ReplicateReconciliationJob. Posao čita po jedno najnovije Raw očitavanje iz svake od 10 tabela i računa prosek. Ako postoji bar jedna vrednost unutar ±Tolerance u odnosu na prosek, bira se vremenski najskorija takva vrednost. Ako ne postoji nijedna u opsegu, bira se vremenski najskorija vrednost uopšte. Izabrana vrednost se upisuje u svih 10 tabela kao novi red sa Source = Reconciled. Vreme upisa je UTC i može da se poravna na početak minuta iz konfiguracije.

Baza ima 10 tabela sa istom šemom: Id, TimestampUtc, ValueCelsius, Source. Odvojene tabele modeluju replike koje se periodično poravnavaju. UTC vreme uklanja probleme sa vremenskim zonama i driftom klijenta. Server određuje šta je „najnovije“.

App.config u ServiceHost drži WCF endpoint i binding, connection string i appSettings (Tolerance, period, poravnanje na minut). _Diagnostics_ su isključeni da bi servis radio bez admin prava.

Zavisnosti: Simulator → Contracts. ServiceHost → Contracts i → Data. Data → EntityFramework. Contracts → .NET WCF a ne zavisi od EF-a. Simulator ne zavisi od Data sloja.

# 3) Organizacija Koda

```
SensorsSolution/
  Sensors.Contracts/          # WCF ugovor (service contract) + DTO poruke
  Sensors.Data/               # EF6 entiteti (10 tabela) + DbContext + migracije
  Sensors.ServiceHost/        # WCF self-host servis + job za poravnanje + App.config
  Sensors.Simulator/          # Klijent: 10 „senzora“ koji šalju očitavanja
```

## 3.1 Sensors.Contracts

Komunikacija između klijenta i servisa. Sadrži service contract (interfejs koji opisuje šta se poziva) i DTO poruke (kako izgleda podatak koji šaljemo). Nema baze ni poslovne logike.

- _ISensorService.cs_ - WCF ugovor (interfejs servisa)

  Deklariše dostupne metode (npr. `SubmitReading`, `Ping`, opciono read-metode).

  ```csharp
  [ServiceContract]
  public interface ISensorService
  {
      [OperationContract(IsOneWay = true)]
      void SubmitReading(SensorReadingDto reading); // klijent šalje, ne čeka odgovor
  }
  ```

- _SensorId.cs_ - identitet senzora (enum S1..S10)

  Jednoznačno označava koji senzor šalje očitavanje.

  ```csharp
  [DataContract]
  public enum SensorId { [EnumMember] S1 = 1, /* ... */ [EnumMember] S10 = 10 }
  ```

- _SensorReadingDto.cs_ - DTO poruka (podatak koji ide preko mreže)

  Struktura sa poljima: koji senzor, koja temperatura, vremenske oznake, izvor.

  ```csharp
  [DataContract]
  public class SensorReadingDto
  {
      [DataMember(Order = 1, IsRequired = true)] public SensorId SensorId { get; set; }
      [DataMember(Order = 2, IsRequired = true)] public double ValueCelsius { get; set; }
      // Server popunjava ServerTimestampUtc; Source: "Raw" ili "Reconciled"
  }
  ```

## 3.2 Sensors.Data

Sloj za podatke (EF6). Definiše šemu baze (10 tabela - po jedna po senzoru), `DbContext` i migracije. Replikaciono poravnanje koristi ove tabele kao replike.

- _Entities/SensorXReading.cs (×10)_ – Po jedan entitet/tabela za svaki senzor

  Modeluje 10 replika (1 klasa = 1 tabela).

  ```csharp
  public class Sensor1Reading
  {
      public int Id { get; set; }
      public DateTime TimestampUtc { get; set; }     // vreme upisa (server)
      public double ValueCelsius { get; set; }       // temperatura
      public string Source { get; set; }             // "Raw" ili "Reconciled"
  }
  ```

- _SensorsDbContext.cs_ – EF6 kontekst sa 10 `DbSet<>`

  Mapira entitete na tabele i podešava konvencije.

  ```csharp
  public class SensorsDbContext : DbContext
  {
      public SensorsDbContext() : base("name=SensorsDb") { } // connection string ime

      public DbSet<Sensor1Reading> Sensor1Readings { get; set; }
      // ... do Sensor10Readings

      protected override void OnModelCreating(DbModelBuilder modelBuilder)
      {
          modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); // klasa == tabela
          modelBuilder.Entity<Sensor1Reading>().Property(p => p.Source).IsRequired().HasMaxLength(20);
          // ... isto za ostale entitete
      }
  }
  ```

- _Sensors.Data/Migrations/`_ – EF migracije

  Reproducibilna šema baze.

  - `Enable-Migrations -ProjectName Sensors.Data -StartUpProjectName Sensors.ServiceHost`
  - `Add-Migration InitialCreate -ProjectName Sensors.Data -StartUpProjectName Sensors.ServiceHost`
  - `Update-Database -ProjectName Sensors.Data -StartUpProjectName Sensors.ServiceHost`

## 3.3 Sensors.ServiceHost

Proces (WCF self-host) sluša na `net.tcp://localhost:9001/SensorService`. Prima `SubmitReading`, server beleži vreme i čuva RAW u bazu. Na svakih 60 s pokreće replikaciono poravnanje (konsenzus) i upisuje Reconciled u svih 10 tabela.

Timestamp je na serveru te se time eliminišu razlike klijentskih satova. „Najnovije“ je uvek po serveru.

- _Hosting/Program.cs_ - Startuje WCF host i tajmer

  ```csharp
  using (var host = new System.ServiceModel.ServiceHost(typeof(SensorService))) // param-less: baseAddress u App.config
  {
      host.Open();
      _timer = new Timer(_ => SafeRun(), null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
      Console.WriteLine("[ServiceHost] Reconciliation timer started (every 60s).");
  }
  ```

  Koristi se parametarski-prazan `ServiceHost` jer je _baseAddress_ u App.config-u. Tako se izbegava greška „already contains an address with scheme net.tcp…“ Dodatno Pošto je naziv projekta `Sensors.ServiceHost`, koristi se puno ime `System.ServiceModel.ServiceHost` da ne dođe do konflikta.

- _Jobs/ReplicateReconciliationJob.cs_ - Logika poravnanja (svakih 60 s)

  ```csharp
  double tolerance = ReadDouble("Tolerance", 5.0);              // ±Tolerance iz appSettings
  var avg = latest.Average(x => x.value);                       // prosek „najnovijih“ 10 RAW
  var inRange = latest.Where(x => Math.Abs(x.value - avg) <= tolerance).ToList();
  var chosen = inRange.Any() ? inRange.OrderByDescending(x => x.ts).First()
                             : latest.OrderByDescending(x => x.ts).First(); // fallback: najnovija ukupno
  var insertTs = ReadBool("AlignToWholeMinute", true) ? AlignToMinute(DateTime.UtcNow) : DateTime.UtcNow;
  // upis „chosen.value“ u SVE tabele sa Source="Reconciled"
  db.SaveChanges();
  ```

- _Services/SensorService.cs_ - Implementacija WCF ugovora
  ```csharp
  var serverNow = DateTime.UtcNow;                              // vreme pečatira server
  db.SensorXReadings.Add(new SensorXReading {                   // zapis RAW u odgovarajuću tabelu
      TimestampUtc = serverNow, ValueCelsius = reading.ValueCelsius, Source = "Raw"
  });
  // ...
  return Project(db.SensorXReadings.OrderByDescending(x => x.TimestampUtc).FirstOrDefault(), SensorId.SX);
  ```

## 3.4 Sensors.Simulator

Konzolni klijent koji simulira 10 senzora. Svaki „radnik“ (task) na 1–10 s generiše nasumičnu temperaturu i poziva servis (`SubmitReading`) preko net.tcp. Simulator nema EF/bazu već priča sa servisom preko WCF ugovora iz `Sensors.Contracts`.

- _Client/Program.cs_ - Pokreće 10 radnika i upravlja gašenjem

  ```csharp
  // Ctrl+C -> otkazivanje svih radnika
  Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

  // Start S1..S10
  for (int i = 1; i <= 10; i++)
      tasks.Add(new SensorWorker((SensorId)i, seedBase + i).RunAsync(cts.Token));

  await Task.WhenAll(tasks);      // sačekaj sve radnike
  WcfClientFactory.Close();       // uredno zatvori WCF resurse
  ```

- _Client/SensorWorker.cs_ - Logika jednog senzora

  ```csharp
  // Pauza 1–10 s između merenja
  var delayMs = _rng.Next(1, 11) * 1000;
  await Task.Delay(delayMs, ct);

  // Nasumična temperatura ~ [18, 30) °C
  var value = 18.0 + _rng.NextDouble() * 12.0;

  // Slanje očitavanja servisu (Source=Raw; server pečatira vreme)
  WcfClientFactory.GetChannel().SubmitReading(new SensorReadingDto {
      SensorId = _sensorId, ValueCelsius = value, ClientTimestampUtc = DateTime.UtcNow, Source = "Raw"
  });
  ```

- _Client/WcfClientFactory.cs_ - Kreira WCF kanal

  ```csharp
  // Jedan zajednički ChannelFactory<ISensorService> za sve radnike
  var binding = new NetTcpBinding();
  var address = new EndpointAddress("net.tcp://localhost:9001/SensorService");
  _factory = new ChannelFactory<ISensorService>(binding, address);

  // (Re)otvori kanal po potrebi
  _channel = _factory.CreateChannel();
  ((IClientChannel)_channel).Open();

  // Uredno zatvaranje (Close/Abort) pri izlasku
  ```

## 4) Pokretanje Projekta

1. Otvoriti rešenje u Visual Studio 2022+ (target: .NET Framework 4.8).
2. Podesiti Multiple startup projects: Sensors.ServiceHost = Start, Sensors.Simulator = Start (ServiceHost prvi).
3. Ctrl+F5.
4. Otvaraju se dva prozora:
   - ServiceHost: „WCF service running…“ + „Reconciliation timer started…“.
   - Simulator: „Starting 10 sensor workers…“ + periodične poruke „Sent S#: XX.XX °C“.

## 5) Rezultati

U nastavku su prikazani rezultati rada sistema. Komunikacija između simulatora i servisa u konzoli, kao i stanje podataka u bazi.

## 5.1 Izlaz u konzoli

![Pregled konzole](https://github.com/boriscu/Consistency/blob/main/images/console_overview.png)

Na levoj strani je ServiceHost koji pokazuje:

- Da je WCF servis uspešno pokrenut na net.tcp://localhost:9001/SensorService,
- Da se reconciliation tajmer aktivira svakih 60 sekundi,
- Log poruke procesa poravnanja (avg=..., chosen=..., ts=...).

Na desnoj strani je Simulator koji prikazuje rad 10 senzora:

- Svaki red prikazuje trenutak kada je određeni senzor poslao temperaturu,
- Vrednosti se kreću u intervalu od ~18 °C do ~30 °C,
- Intervali slanja su nasumični (1–10 sekundi), što se vidi po neujednačenim vremenskim oznakama.

## 5.2 Podaci u bazi

![Pregled baze](https://github.com/boriscu/Consistency/blob/main/images/database_overview.png)

Na slici su prikazane dve tabele iz baze (Sensor1Reading i Sensor10Reading). Vidimo sledeće obrasce:

- RAW redovi: Pojedinačna očitavanja senzora, koja imaju oznaku Source = Raw. Njih generiše simulator na svaka 1–10 sekundi.
- RECONCILED redovi: Na svakih 60 sekundi dodaje se novi red sa Source = Reconciled. Ta vrednost je rezultat poravnanja:
  - U svim tabelama se upisuje ista vrednost,
  - Timestamp je poravnat na početak minuta,
  - Ta vrednost predstavlja „konsenzus“ između najnovijih RAW očitavanja.

U obe tabele se vidi red sa Source = Reconciled za isti vremenski trenutak (09:33:00 UTC), i vrednost je ista, što potvrđuje da se mehanizam poravnanja uspešno izvršava i sinhronizuje sva ocitavanja.
