using System;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using ServiceStack;

namespace MittoSms.ServiceInterface
{
    public class SMSServices: Service
    {
        public SendSMSResponse Get(SendSMS request)
        {
            if (request.From.IsNullOrEmpty())
                throw new ArgumentException("'From' argument is required");

            if (request.To.IsNullOrEmpty())
                throw new ArgumentException("'To' argument is required");

            if (request.Text.IsNullOrEmpty())
                throw new ArgumentException("'Text' argument is required");

            return new SendSMSResponse
            {
                State = SendSMSResponseState.Success
            };
        }
    }
}
