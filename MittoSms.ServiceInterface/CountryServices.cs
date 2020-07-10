using System;
using System.Collections.Generic;
using MittoSms.ServiceModel;
using MittoSms.ServiceModel.Types;
using ServiceStack;
using ServiceStack.OrmLite;

namespace MittoSms.ServiceInterface
{
    public class CountryServices : Service
    {
        public List<Country> Get(Countries request)
        {
            return Db.Select<Country>();
        }
    }
}
