using System;
using System.Collections.Generic;
using MittoSms.ServiceModel.Types;
using ServiceStack;

namespace MittoSms.ServiceModel
{
    [Route("/sms/send","GET")]
    public class SendSMS: IReturn<SendSMSResponse>
    {
        public String From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
    }

    public class SendSMSResponse
    {
        public SendSMSResponseState State { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/sms/sent", "GET")]
    public class GetSentSMS: IReturn<GetSentSMSResponse>
    {
        public String DateTimeFrom { get; set; }
        public String DateTimeTo { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class GetSentSMSResponse
    {
        public long TotalCount { get; set; }
        public List<Sms> Items { get; set; } 
    }
}
