using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Payment.Stripe
{
    public class FirstPaymentData
    {
        public string CountryCode { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string OwnerName { get; set; }
        public string Descripion { get; set; }
        public string RedirectReturnUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public abstract class PaymentSourceCreator
    {
        public static PaymentSourceCreator Build(FirstPaymentData data)
        {
            if ("BE".Equals(data.CountryCode?.ToUpperInvariant())) return new BePaymentSource();
            if ("DE".Equals(data.CountryCode?.ToUpperInvariant())) return new DePaymentSource();
            if ("NL".Equals(data.CountryCode?.ToUpperInvariant())) return new NlPaymentSource();
            // sofort as default first payment for now
            // country supported: Austria, Spain
            return new OtherEuropePaymentSource();
        }

        public abstract StripeSource Create(StripeSettings stripeSettings, FirstPaymentData data);
    }

    public class NlPaymentSource : PaymentSourceCreator
    {
        public override StripeSource Create(StripeSettings stripeSettings, FirstPaymentData data)
        {
            var source = new StripeSourceService(stripeSettings.StripePrivateKey).Create(new StripeSourceCreateOptions
            {
                Type = StripeSourceType.Ideal,
                Amount = data.Amount,
                Currency = data.Currency,
                Owner = new StripeSourceOwner
                {
                    Name = data.OwnerName
                },
                RedirectReturnUrl = data.RedirectReturnUrl,
                Metadata = data.Metadata,
                IdealStatementDescriptor = data.Description
            });
            return source;
        }
    }

    public class BePaymentSource : PaymentSourceCreator
    {
        public override StripeSource Create(StripeSettings stripeSettings, FirstPaymentData data)
        {
            var source = new StripeSourceService(stripeSettings.StripePrivateKey).Create(new StripeSourceCreateOptions
            {
                Type = StripeSourceType.Bancontact,
                Amount = data.Amount,
                Currency = data.Currency,
                Owner = new StripeSourceOwner
                {
                    Name = data.OwnerName
                },
                RedirectReturnUrl = data.RedirectReturnUrl,
                Metadata = data.Metadata
            });
            return source;
        }
    }

    public class OtherEuropePaymentSource : PaymentSourceCreator
    {
        public override StripeSource Create(StripeSettings stripeSettings, FirstPaymentData data)
        {
            var source = new StripeSourceService(stripeSettings.StripePrivateKey).Create(new StripeSourceCreateOptions
            {
                Type = StripeSourceType.Sofort,
                Amount = data.Amount,
                Currency = data.Currency,
                Owner = new StripeSourceOwner
                {
                    Name = data.OwnerName
                },
                RedirectReturnUrl = data.RedirectReturnUrl,
                SofortCountry = data.CountryCode,
                SofortStatementDescriptor = "", // define statement description
                Metadata = data.Metadata
            });
            return source;
        }
    }

    public class DePaymentSource : PaymentSourceCreator
    {
        public override StripeSource Create(StripeSettings stripeSettings, FirstPaymentData data)
        {
            var source = new StripeSourceService(stripeSettings.StripePrivateKey).Create(new StripeSourceCreateOptions
            {
                Type = StripeSourceType.Giropay,
                Amount = data.Amount,
                Currency = data.Currency,
                Owner = new StripeSourceOwner
                {
                    Name = data.OwnerName
                },
                RedirectReturnUrl = data.RedirectReturnUrl,
                Metadata = data.Metadata
            });
            return source;
        }
    }
}
