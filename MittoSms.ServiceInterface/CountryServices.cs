using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using ServiceStack;
using ServiceStack.OrmLite;

namespace MittoSms.ServiceInterface
{
    public class CountryServices : Service
    {
        public async Task<List<Country>> Get(Countries request)
        {
            return await Db.SelectAsync<Country>();
        }
    }
}
