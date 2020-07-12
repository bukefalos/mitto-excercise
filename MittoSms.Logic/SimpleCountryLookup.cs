using System;
using System.Collections.Generic;
using MittoSms.ServiceModel.Types;

namespace MittoSms.Logic
{
    public class SimpleCountryLookup : ICountryLookup
    {
        readonly int CallingCodeCheckLength;

        public SimpleCountryLookup(int firstDigitsToCheck)
        {
            this.CallingCodeCheckLength = firstDigitsToCheck;
        }

        public Country ByPhoneNumber(List<Country> countries, string phoneNumber)
        {
            for (int callingCodeLength = CallingCodeCheckLength; callingCodeLength > 0; callingCodeLength--)
            {
                var potentialCallingCode = phoneNumber.Substring(0, callingCodeLength);
                var foundCountry = countries.Find(country => country.Cc.Equals(potentialCallingCode));
                if (foundCountry != null)
                {
                    return foundCountry;
                }
            }
            throw new ArgumentException("Unsupported receiver calling code");
        }
    }
}
