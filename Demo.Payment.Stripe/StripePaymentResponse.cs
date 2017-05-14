using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Payment.Stripe
{
    public class StripePaymentResponse
    {
        public string currency { get; set; }
        public int amount { get; set; }
        public string customerid { get; set; }
        public string source { get; set; }
        public string returnUrl { get; set; }
        public string status { get; set; }
    }
}
