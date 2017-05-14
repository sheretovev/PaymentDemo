using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Payment.Stripe
{
    public class StripePaymentRequest
    {
        public string CountryCode { get; set; }
        public string Email { get; set; }
        public int AmountCents { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string ReturnUrl { get; set; }
        public IDictionary<string, string> ReturnUrlParams { get; set; }
        public string RedirectUrl { get; set; }
        public string CityOrTown { get; set; }
        public string Line1 { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Iban { get; set; }
    }
}
