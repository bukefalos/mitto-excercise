using System;
using ServiceStack.DataAnnotations;

namespace MittoSms.ServiceModel.Types
{
    [Alias("sms")]
    public class Sms
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Required]
        public String From { get; set; }

        [Required]
        public String To { get; set; }

        [Required]
        public String Text { get; set; }

        [Required]
        [Alias("created_at")]
        public DateTime CreatedAt { get; set; }

        public SentSMSState State { get; set; }

        [Required]
        public Decimal Price { get; set; }

        [Reference]
        public Country Country { get; set; }

        [Required]
        [Alias("country_id")]
        [References(typeof(Country))]
        public int CountryId { get; set; }
    }
}
