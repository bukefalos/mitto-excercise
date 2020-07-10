using System;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using ServiceStack;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;

namespace MittoSms.ServiceInterface
{

    public class SendSMSValidator : AbstractValidator<SendSMS>
    {
        public SendSMSValidator()
        {
            RuleFor(x => x.Text).NotEmpty().WithMessage("A Text is what's needed.");
            RuleFor(x => x.From).NotEmpty().WithMessage("A From is what's needed.");
            RuleFor(x => x.To).NotEmpty().WithMessage("A To is what's needed.");
        }
    }

    public class SMSServices : Service
    {
        public SendSMSResponse Get(SendSMS request)
        {
            if (request.From.IsNullOrEmpty())
                throw new ArgumentException("'From' argument is required");

            if (request.To.IsNullOrEmpty())
                throw new ArgumentException("'To' argument is required");

            if (request.Text.IsNullOrEmpty())
                throw new ArgumentException("'Text' argument is required");


            var sms = request.ConvertTo<Sms>();
            sms.Created = DateTime.Now;

            Db.Save(sms);
            return new SendSMSResponse
            {
                State = SendSMSResponseState.Success
            };
        }

        public GetSentSMSResponse Get(GetSentSMS request)
        {
            var totalSmsRecords = Db.Count<Sms>();
            var smsRecords = Db.Select(Db.From<Sms>()
                .Skip(request.Skip)
                .Limit(request.Take)
            );

            return new GetSentSMSResponse()
            {
                Items = smsRecords,
                TotalCount = totalSmsRecords
            };
        }
    }
}
