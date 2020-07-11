using System;
using System.Collections.Generic;
using ServiceStack;

namespace MittoSms.ServiceModel
{
    [Route("statistics", "GET")]
    public class Statistics: IReturn<List<StatisticsRecord>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<String> MccList { get; set; }
    }

    public class StatisticsRecord
    {
        public DateTime Day { get; set; }
        public string Mcc { get; set; }
        public Decimal PricePerSms { get; set; }
        public Decimal TotalPrice { get; set; }
    }
}
