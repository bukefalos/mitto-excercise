using System;
using ServiceStack.DataAnnotations;

namespace MittoSms.ServiceModel.Types
{
    [Alias("country")]
    public class Country
    {
        [AutoIncrement]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Mcc { get; set; }

        [Required]
        public string Cc { get; set; }

        [Required]
        [Alias("price_per_sms")]
        public Decimal PricePerSMS { get; set; }
    }
}
