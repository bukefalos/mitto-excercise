using System;
using ServiceStack.DataAnnotations;

namespace MittoSms.ServiceModel.Types
{
    public class Country
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Mcc { get; set; }

        public string Cc { get; set; }

        [Alias("price_per_sms")]
        public Decimal PricePerSMS { get; set; }
    }
}
