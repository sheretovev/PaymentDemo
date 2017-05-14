using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Payment.Adyen
{
    public class AdyenPaymentRequest
    {
        public string Email { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string Name { get; set; }
        public string ReturnUrl { get; set; }
        public IDictionary<string, string> ReturnUrlParams { get; set; }
    }
}
