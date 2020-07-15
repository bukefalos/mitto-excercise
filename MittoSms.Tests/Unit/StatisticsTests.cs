using System;
using System.Collections.Generic;
using System.Data;
using MittoSms.ServiceInterface;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Testing;
using ServiceStack.Text;

namespace MittoSms.Tests.Unit
{
    public class StatisticsTests
    {
        
        private readonly ServiceStackHost appHost;


        public StatisticsTests()
        {
            appHost = new BasicAppHost(typeof(StatisticsServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    ;
                    container.Register<IDbConnectionFactory>(c =>
                        new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));

                    using (var db = container.TryResolve<IDbConnectionFactory>().Open())
                    {
                        
                        db.DropAndCreateTable<Country>();
                        db.Insert(new Country { Name = "Germany", Mcc = "262", Cc = "49", PricePerSMS = 0.55m });
                        db.Insert(new Country { Name = "Austria", Mcc = "232", Cc = "43", PricePerSMS = 0.53m });
                        db.Insert(new Country { Name = "Poland", Mcc = "260", Cc = "48", PricePerSMS = 0.32m });
                    }

                    JsConfig.AlwaysUseUtc = true;
                }
            }
            .Init();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            using (var db = appHost.TryResolve<IDbConnectionFactory>().Open())
            {
                db.DropAndCreateTable<Sms>();
            }
        }

        [Test]
        public void Statistics_Without_Mcc_Filter()
        {
            using (var db = appHost.TryResolve<IDbConnectionFactory>().Open())
            using (var service = appHost.TryResolve<StatisticsServices>())
            {
                var germany = db.Single<Country>(c => c.Name == "Germany");
                var poland = db.Single<Country>(c => c.Name == "Poland");
                InsertSms(db, germany, "2020-07-13T15:00:00");
                InsertSms(db, poland, "2020-07-13T22:00:00");

                var result = service.Get(new Statistics {
                    DateFrom = "2020-07-13",
                    DateTo = "2020-07-13"
                }).GetResult();
                var response = result.Response.ConvertTo<List<StatisticsRecord>>();

                Assert.That(response.Count, Is.EqualTo(2));
                Assert.That(response[0].Day, Is.EqualTo(Convert.ToDateTime("2020-07-13T00:00:00")));
                Assert.That(response[0].Mcc, Is.Not.EqualTo(response[1].Mcc));
                Assert.That(response[1].Day, Is.EqualTo(Convert.ToDateTime("2020-07-13T00:00:00")));
            }
        }

        [Test]
        public void Statistics_With_Mcc_Filter()
        {
            using (var db = appHost.TryResolve<IDbConnectionFactory>().Open())
            using (var service = appHost.TryResolve<StatisticsServices>())
            {
                var germany = db.Single<Country>(c => c.Name == "Germany");
                var poland = db.Single<Country>(c => c.Name == "Poland");
                InsertSms(db, germany, "2020-07-13T15:00:00");
                InsertSms(db, poland, "2020-07-13T22:00:00");

                var result = service.Get(new Statistics
                {
                    DateFrom = "2020-07-13",
                    DateTo = "2020-07-13",
                    MccList = new List<string> { poland.Mcc }
                }).GetResult();
                var response = result.Response.ConvertTo<List<StatisticsRecord>>();

                Assert.That(response.Count, Is.EqualTo(1));
                Assert.That(response[0].Mcc, Is.EqualTo(poland.Mcc));
            }
        }

        public void Statistics_Sums_Price()
        {
            using (var db = appHost.TryResolve<IDbConnectionFactory>().Open())
            using (var service = appHost.TryResolve<StatisticsServices>())
            {
                var germany = db.Single<Country>(c => c.Name == "Germany");
                InsertSms(db, germany, "2020-07-13T15:00:00");
                InsertSms(db, germany, "2020-07-13T16:00:00");
                InsertSms(db, germany, "2020-07-13T17:00:00");
                InsertSms(db, germany, "2020-07-13T18:00:00");
                InsertSms(db, germany, "2020-07-13T19:00:00");
                InsertSms(db, germany, "2020-07-13T20:00:00");

                var result = service.Get(new Statistics
                {
                    DateFrom = "2020-07-13",
                    DateTo = "2020-07-13",
                    MccList = new List<string> { germany.Mcc }
                }).GetResult();
                var response = result.Response.ConvertTo<List<StatisticsRecord>>();

                Assert.That(response.Count, Is.EqualTo(1));
                Assert.That(response[0].PricePerSms, Is.EqualTo(germany.PricePerSMS));
                Assert.That(response[0].TotalPrice, Is.EqualTo(germany.PricePerSMS * response.Count));
            }
        }

        private static void InsertSms(IDbConnection db, Country country, String createdAt)
        {
            db.Insert(new Sms
            {
                CountryId = country.Id,
                CreatedAt = DateTime.SpecifyKind(Convert.ToDateTime(createdAt), DateTimeKind.Utc),
                Price = country.PricePerSMS,
                State = SentSMSState.Success,
                Text = "Hello",
                From = "Sender",
                To = "+{0}123456789".Fmt(country.Cc)
            });
        }
    }
}
