using System;
using ServiceStack.DataAnnotations;

namespace MittoSms.ServiceModel.Types
{
    public class Sms
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public String From { get; set; }

        public String To { get; set; }

        public String Text { get; set; }

        [Alias("created_at")]
        public DateTime CreatedAt { get; set; }

        public SentSMSState State { get; set; }

        public Decimal Price { get; set; }

        [Reference]
        public Country Country { get; set; }

        [Alias("country_id")]
        [References(typeof(Country))]
        public int CountryId { get; set; }
    }
}
