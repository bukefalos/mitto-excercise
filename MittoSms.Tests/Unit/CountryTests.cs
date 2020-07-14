using MittoSms.ServiceInterface;
using MittoSms.ServiceModel.Types;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Testing;

namespace MittoSms.Tests
{
    public class CountryTests
    {
        private readonly ServiceStackHost appHost;

        public CountryTests()
        {
            appHost = new BasicAppHost(typeof(CountryServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    container.Register<IDbConnectionFactory>(c =>
                        new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));

                    using (var db = container.TryResolve<IDbConnectionFactory>().Open())
                    {
                        db.DropAndCreateTable<Country>();
                        db.Insert(new Country { Name = "Germany", Mcc = "262", Cc = "49", PricePerSMS = 0.55m });
                        db.Insert(new Country { Name = "Austria", Mcc = "232", Cc = "43", PricePerSMS = 0.53m });
                        db.Insert(new Country { Name = "Poland", Mcc = "260", Cc = "48", PricePerSMS = 0.32m });
                    }
                }
            }
            .Init();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        [Test]
        public void Can_Get_Countries()
        {
            using (var service = appHost.TryResolve<CountryServices>())
            {
                var response = service.Get(new ServiceModel.Countries()).GetResult();
                Assert.That(response.Count, Is.EqualTo(3));
                Assert.That(response.Map(c => c.Name), Is.EquivalentTo(new[] { "Germany", "Austria", "Poland" }));
            }
        }
    }
}