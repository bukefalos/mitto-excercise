using System;
using ServiceStack;
using ServiceStack.Text;

namespace MittoSms.ServiceInterface
{
    public static class Extensions
    {
        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }

    }

    public static class Utils
    {
        public static HttpResult UsingDateFormatOnly(object response)
        {
            return new HttpResult(response)
            {
                ResultScope = () =>
                    JsConfig.With(new Config { DateHandler = DateHandler.ISO8601DateOnly })
            };
        }
    }

    
}
