using System;
using System.Collections.Generic;
using MittoSms.ServiceModel.Types;

namespace MittoSms.Logic
{
    public interface ICountryLookup
    {
        Country ByPhoneNumber(List<Country> countries, String phoneNumber);
    }
}
