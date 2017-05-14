using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Demo.Payment.Adyen
{
    public class AdyenPaymentProvider
    {
        public const string AdyenDateFormat = "yyyy-MM-ddTHH:mm:ssZ";

        private readonly AdyenSettings _settings;

        public AdyenPaymentProvider(AdyenSettings settings)
        {
            _settings = settings;
        }

        private string AdyenUrl = "https://test.adyen.com/hpp/pay.shtml";
        public string BuildLink(SortedList<string, string> list)
        {
            var key = _settings.HmacKey;

            var messageToSign = string.Concat(string.Join(":", list.Keys), ":", string.Join(":", list.Values.Select(x => x.Replace("\\", "\\\\").Replace(":", "\\:"))));
            var sign = new HmacHelper().CalculateHashHMACHex(key, messageToSign);

            var parameters = string.Join("&", list.Select(x => $"{x.Key}={x.Value}"));
            parameters = $"{parameters}&merchantSig={WebUtility.UrlEncode(sign)}";
            return $"{AdyenUrl}?{parameters}";
        }
    }
}
