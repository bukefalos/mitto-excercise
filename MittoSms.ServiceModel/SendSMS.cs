using System;
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
}
