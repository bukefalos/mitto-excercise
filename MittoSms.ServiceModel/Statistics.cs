﻿using System;
using System.Collections.Generic;
using ServiceStack;

namespace MittoSms.ServiceModel
{
    [Route("/statistics", "GET")]
    public class Statistics: IReturn<List<StatisticsRecord>>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public List<String> MccList { get; set; }
    }

    public class StatisticsRecord
    {
        public DateTime Day { get; set; }
        public String Mcc { get; set; }
        public Decimal PricePerSms { get; set; }
        public Int32 Count { get; set; }
        public Decimal TotalPrice { get; set; }
    }
}
