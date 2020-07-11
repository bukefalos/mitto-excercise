using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        public async Task<SendSMSResponse> Get(SendSMS request)
        {
            //TODO: select country which receiver belongs to (maybe move to ServiceLogic)
            var receiverCountry = FindCountry(
                await Db.SelectAsync<Country>(),
                request.To.ReplaceAll("+", ""));
                        
            var sms = request.ConvertTo<Sms>();
            sms.CreatedAt = DateTime.Now.ToUniversalTime();
            sms.State = SentSMSState.Success;
            sms.Price = receiverCountry.PricePerSMS;
            sms.CountryId = receiverCountry.Id;

            await Db.SaveAsync(sms);
            return new SendSMSResponse
            {
                State = sms.State
            };
        }

        private Country FindCountry(List<Country> countries, String receiver)
        {
            for (int callingCodeLength = 4; callingCodeLength > 0; callingCodeLength--)
            {
                var potentialCallingCode = receiver.Substring(0, callingCodeLength);
                var foundCountry = countries.Find(country => country.Cc.Equals(potentialCallingCode));
                if (foundCountry != null)
                {
                    return foundCountry;
                }
            }
            throw new ArgumentException("Unsupported receiver calling code");
        }

        public async Task<GetSentSMSResponse> Get(GetSentSMS request)
        {
            //TODO: request validation
            var from = Convert.ToDateTime(request.DateTimeFrom);
            var to = Convert.ToDateTime(request.DateTimeTo);
            Expression<Func<Sms, bool>> dateTimeCondition = sms => from <= sms.CreatedAt && sms.CreatedAt >= to;

            var totalSmsRecords = await Db.CountAsync<Sms>(dateTimeCondition);
            var smsRecords = await Db.SelectAsync(Db.From<Sms>()
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
