using System;
using System.Linq.Expressions;
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
            sms.Created = DateTime.Now.ToUniversalTime();

            Db.Save(sms);
            return new SendSMSResponse
            {
                State = SendSMSResponseState.Success
            };
        }

        public GetSentSMSResponse Get(GetSentSMS request)
        {
            var from = Convert.ToDateTime(request.DateTimeFrom);
            var to = Convert.ToDateTime(request.DateTimeTo);
            Expression<Func<Sms, bool>> dateTimeCondition = sms => from <= sms.Created && sms.Created >= to;

            var totalSmsRecords = Db.Count<Sms>(dateTimeCondition);
            var smsRecords = Db.Select(Db.From<Sms>()
                .Where(dateTimeCondition)
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
