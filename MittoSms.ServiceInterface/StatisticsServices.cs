using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.Text;

namespace MittoSms.ServiceInterface
{
    public class StatisticsServices : Service
    {
        
        public async Task<HttpResult> Get(Statistics request)
        {
            var from = Convert.ToDateTime(request.DateFrom);
            var to = Convert.ToDateTime(request.DateTo).EndOfDay();
            var query = Db.From<Sms>()
                    .Join<Sms, Country>()
                    .Select<Sms, Country>((sms, country) => new
                    {
                        Mcc = country.Mcc,
                        Day = Sql.Cast(sms.CreatedAt, "DATE"),
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
