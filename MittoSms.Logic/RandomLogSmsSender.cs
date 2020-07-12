using System;
using System.Threading.Tasks;
using MittoSms.ServiceModel.Types;
using ServiceStack.Logging;

namespace MittoSms.Logic
{
    public class RandomLogSmsSender : ISmsSender
    {
        public static ILog Log = LogManager.GetLogger(typeof(ISmsSender));
        private readonly Random Random = new Random();

        public Task<SentSMSState> Send(string sender, string receiver, string text)
        {
            Log.InfoFormat("Trying to send sms from {0} with text '{1}' to {2}", sender, text, receiver);
            var randomInteger = Random.Next(0, 10);
            return randomInteger > 7
                ? Task.FromResult(SentSMSState.Failed)
                : Task.FromResult(SentSMSState.Success);
        }
    }
}
