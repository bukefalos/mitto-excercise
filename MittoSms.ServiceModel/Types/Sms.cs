using System;
using ServiceStack.DataAnnotations;

namespace MittoSms.ServiceModel.Types
{
    public class Sms
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }
}
