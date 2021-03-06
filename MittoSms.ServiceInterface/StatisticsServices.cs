﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using ServiceStack;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
using ServiceStack.Text;

namespace MittoSms.ServiceInterface
{
    public class StatisticsRequestValidator : AbstractValidator<Statistics>
    {
        public StatisticsRequestValidator()
        {
            RuleFor(x => x.DateFrom).NotEmpty().WithMessage("Parameter 'dateFrom' is mandatory.");
            RuleFor(x => x.DateTo).NotEmpty().WithMessage("Paramter 'dateTo' is mandatory");
        }
    }

    public class StatisticsServices : Service
    {
        
        public async Task<HttpResult> Get(Statistics request)
        {
            var from = DateTime.SpecifyKind(Convert.ToDateTime(request.DateFrom), DateTimeKind.Utc);
            var to = DateTime.SpecifyKind(Convert.ToDateTime(request.DateTo), DateTimeKind.Utc).EndOfDay();
            var query = Db.From<Sms>()
                    .Join<Sms, Country>()
                    .Select<Sms, Country>((sms, country) => new
                    {
                        Mcc = country.Mcc,
                        Day = Sql.Custom("DATE(created_at)"),
                        Count = Sql.Count("*"),
                        PricePerSms = Sql.Max(country.PricePerSMS),
                        TotalPrice = Sql.Sum(sms.Price)
                    })
                    .Where<Sms>(sms => from <= sms.CreatedAt && sms.CreatedAt <= to)
                    .And<Country>(country => request.MccList.IsNullOrEmpty() || Sql.In(country.Mcc, request.MccList))
                    .GroupBy(sms => new { Day = Sql.Custom("DATE(created_at)"), sms.Country.Mcc });

            return Utils.UsingDateFormatOnly(await Db.SelectAsync<StatisticsRecord>(query));
        }
    }

}
