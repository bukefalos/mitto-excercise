using System;
using System.Threading.Tasks;
using MittoSms.Logic;
using MittoSms.ServiceInterface;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Testing;

namespace MittoSms.Tests.Unit
{
    public class SmsTests
    {
        private readonly ServiceStackHost appHost;


        public class SuccessSmsSender : ISmsSender
        {
            public Task<SentSMSState> Send(string sender, string receiver, string text)
            {
                return Task.FromResult(SentSMSState.Success);
            }
        }

        public SmsTests()
        {
            appHost = new BasicAppHost(typeof(SMSServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    container.Register<ICountryLookup>(c => new SimpleCountryLookup(4));
                    container.Register<ISmsSender>(c => new SuccessSmsSender());

                    container.Register<IDbConnectionFactory>(c =>
                        new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));
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
                db.DropAndCreateTable<Country>();
                db.DropAndCreateTable<Sms>();
                db.Insert(new Country { Name = "Germany", Mcc = "262", Cc = "49", PricePerSMS = 0.55m });
                db.Insert(new Country { Name = "Austria", Mcc = "232", Cc = "43", PricePerSMS = 0.53m });
                db.Insert(new Country { Name = "Poland", Mcc = "260", Cc = "48", PricePerSMS = 0.32m });
            }
        }

        [Test]
        public void Get_Sent_SMS_Date_Filter()
        {
            using (var db = appHost.TryResolve<IDbConnectionFactory>().Open())
            using (var service = appHost.TryResolve<SMSServices>())
            {
                var germany = db.Single<Country>(c => c.Name == "Germany");
                db.Insert(new Sms
                {
                    CountryId = germany.Id,
                    CreatedAt = DateTime.SpecifyKind(Convert.ToDateTime("2020-07-13T15:00:00Z"), DateTimeKind.Utc),
                    Price = germany.PricePerSMS,
                    State = SentSMSState.Success,
                    Text = "Hello",
                    From = "Sender",
                    To = "+{0}123456789".Fmt(germany.Cc)
                });

                var poland = db.Single<Country>(c => c.Name == "Poland");
                db.Insert(new Sms
                {
                    CountryId = poland.Id,
                    CreatedAt = DateTime.SpecifyKind(Convert.ToDateTime("2020-07-13T22:00:00Z"), DateTimeKind.Utc),
                    Price = poland.PricePerSMS,
                    State = SentSMSState.Success,
                    Text = "Hello",
                    From = "Sender",
                    To = "+{0}123456789".Fmt(poland.Cc)
                });

                var response = service.Get(new GetSentSMS { DateTimeFrom = "2020-07-13T10:00:00Z", DateTimeTo = "2020-07-13T20:00:00Z" }).GetResult();
                Assert.That(response.TotalCount, Is.EqualTo(1));
                Assert.That(response.Items[0].CountryId, Is.EqualTo(germany.Id));
            }
        }


        [Test]
        public void Get_Send_SMS_Creates_SMS_DB_entry()
        {
            using (var db = appHost.TryResolve<IDbConnectionFactory>().Open())
            using (var service = appHost.TryResolve<SMSServices>())
            {
                var austria = db.Single<Country>(c => c.Name == "Austria");
                var goodLuckSms = new SendSMS {
                    From = "The Sender",
                    To = "+{0}123456789".Fmt(austria.Cc),
                    Text = "Hello, good luck with mitto excercise" };

                var response = service.Get(goodLuckSms).GetResult();
                Assert.That(response.State, Is.EqualTo(SentSMSState.Success));

                var smsFromDb = db.Single<Sms>(s => s.Text == goodLuckSms.Text);
                Assert.That(smsFromDb.CountryId, Is.EqualTo(austria.Id));
                Assert.That(smsFromDb.Price, Is.EqualTo(austria.PricePerSMS));
            }
        }
    }
}
