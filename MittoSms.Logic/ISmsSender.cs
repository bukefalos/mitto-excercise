using System;
using System.Threading.Tasks;
using MittoSms.ServiceModel.Types;

namespace MittoSms.Logic
{
    public interface ISmsSender
    {
        Task<SentSMSState> Send(string sender, string receiver, string text);
    }
}
