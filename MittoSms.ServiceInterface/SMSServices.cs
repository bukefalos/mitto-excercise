using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using MittoSms.Logic;
using ServiceStack;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
using ServiceStack.Text;

namespace MittoSms.ServiceInterface
{

    public class SendSMSValidator : AbstractValidator<SendSMS>
    {
        public SendSMSValidator()
        {
            RuleFor(x => x.Text).NotEmpty().WithMessage("Parameter 'Text' is mandatory.");
            RuleFor(x => x.From).NotEmpty().WithMessage("Paramter 'From' is mandatory");
            RuleFor(x => x.To).NotEmpty().WithMessage("Parameter 'To' is mandatory.");
        }
    }

    public class GetSentSMSValidator : AbstractValidator<GetSentSMS>
    {
        public GetSentSMSValidator()
        {
            RuleFor(x => x.DateTimeFrom).NotEmpty().WithMessage("Parameter 'dateTimeFrom' is mandatory.");
            RuleFor(x => x.DateTimeTo).NotEmpty().WithMessage("Paramter 'dateTimeTo' is mandatory");
        }
    }

    public class SMSServices : Service
    {
        readonly ICountryLookup CountryLookup;
        readonly ISmsSender SmsSender;

        public SMSServices(ICountryLookup CountryLookup, ISmsSender SmsSender)
        {
            this.CountryLookup = CountryLookup;
            this.SmsSender = SmsSender;
        }

        public async Task<SendSMSResponse> Get(SendSMS request)
        {
            var receiverCountry = CountryLookup.ByPhoneNumber(
                await Db.SelectAsync<Country>(),
                request.To.ReplaceAll("+", ""));

            var sentState = await SmsSender.Send(request.From, request.To, request.Text);

            var sms = request.ConvertTo<Sms>();
            sms.CreatedAt = DateTime.Now.ToUniversalTime();
            sms.State = sentState;
            sms.Price = receiverCountry.PricePerSMS;
            sms.CountryId = receiverCountry.Id;

            await Db.SaveAsync(sms);
            return new SendSMSResponse
            {
                State = sms.State
            };
        }

        public async Task<GetSentSMSResponse> Get(GetSentSMS request)
        {
            var take = request.Take ?? 10;
            var skip = request.Skip ?? 0;
            var from = DateTime.SpecifyKind(Convert.ToDateTime(request.DateTimeFrom), DateTimeKind.Utc);
            var to = DateTime.SpecifyKind(Convert.ToDateTime(request.DateTimeTo), DateTimeKind.Utc);
            Expression<Func<Sms, bool>> dateTimeCondition = sms => from <= sms.CreatedAt && sms.CreatedAt <= to;

            var totalSmsRecords = await Db.CountAsync<Sms>(dateTimeCondition);
            var smsRecords = await Db.SelectAsync(Db.From<Sms>()
                .Where(dateTimeCondition)
                .Skip(skip)
                .Limit(take)
            );

            return new GetSentSMSResponse()
            {
                Items = smsRecords,
                TotalCount = totalSmsRecords
            };
        }
    }
}
