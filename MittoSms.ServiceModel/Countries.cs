using System;
using System.Collections.Generic;
using MittoSms.ServiceModel.Types;
using ServiceStack;

namespace MittoSms.ServiceModel
{
    [Route("/countries", "GET")]
    public class Countries : IReturn<List<Country>>
    {
    }
}
