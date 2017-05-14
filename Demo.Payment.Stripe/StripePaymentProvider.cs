using System.Collections.Generic;
using Stripe;
using System.Linq;
using System;

namespace Demo.Payment.Stripe
{
    public class StripePaymentProvider
    {
        private readonly StripeSettings _settings;


        public StripePaymentProvider(StripeSettings settings)
        {
            _settings = settings;
        }

        public StripePlan GetPlan(string id)
        {
            try
            {
                return new StripePlanService(_settings.StripePrivateKey).Get(id);
            }
            catch (StripeException ex)
            {
                return null;
            }
        }

        //public IEnumerable<StripePlan> GetPlans()
        //{
        //    return new StripePlanService(_settings.StripePrivateKey).List();
        //}

        //public StripePlan CreatePlan(StripePlanCreateOptions createOptions)
        //{
        //    var plan = new StripePlanService(_settings.StripePrivateKey).Create(createOptions);
        //    return plan;
        //}

        //public StripeSubscription CreateSubscriptions(string customerId, string planId, StripeSubscriptionCreateOptions options)
        //{
        //    var subscription = new StripeSubscriptionService(_settings.StripePrivateKey).Create(customerId, planId, options);
        //    return subscription;
        //}

        //public StripeSource CreateIdealSource(StripePaymentRequest request)
        //{
        //    var source = new StripeSourceService(_settings.StripePrivateKey).Create(new StripeSourceCreateOptions
        //    {
        //        Type = StripeSourceType.Ideal,
        //        Amount = request.AmountCents,
        //        Currency = request.Currency,
        //        Owner = new StripeSourceOwner
        //        {
        //            Name = request.Name,
        //        },
        //        RedirectReturnUrl = request.ReturnUrl,
        //        Flow = StripeSourceFlow.Redirect
        //    });
        //    return source;
        //}

        //public StripeSource CreateBancontactSource(StripePaymentRequest request)
        //{
        //    var source = new StripeSourceService(_settings.StripePrivateKey).Create(new StripeSourceCreateOptions
        //    {
        //        Type = StripeSourceType.Bancontact,
        //        Amount = request.AmountCents,
        //        Currency = request.Currency,
        //        Owner = new StripeSourceOwner
        //        {
        //            Name = request.Name,
        //        },
        //        RedirectReturnUrl = request.ReturnUrl,
        //        IdealBank = request.CountryCode,
        //        SofortStatementDescriptor = "test statement description"
        //    });
        //    return source;
        //}

        //public StripeSource CreateSofortSource(StripePaymentRequest request)
        //{
        //    var source = new StripeSourceService(_settings.StripePrivateKey).Create(new StripeSourceCreateOptions
        //    {
        //        Type = StripeSourceType.Sofort,
        //        Amount = request.AmountCents,
        //        Currency = request.Currency,
        //        Owner = new StripeSourceOwner
        //        {
        //            Name = request.Name,
        //        },
        //        RedirectReturnUrl = request.ReturnUrl,
        //        SofortCountry = request.CountryCode,
        //        SofortStatementDescriptor = "test statement description"
        //    });
        //    return source;
        //}

        //public StripeSource CreateSource(StripeSourceCreateOptions options)
        //{
        //    var source = new StripeSourceService(_settings.StripePrivateKey).Create(options);
        //    return source;
        //}
        //public StripeCharge CreateCharge(ChargeRequest chargeRequest)
        //{
        //    var charge = new StripeChargeService(_settings.StripePrivateKey).Create(new StripeChargeCreateOptions
        //    {
        //        Amount = chargeRequest.Amount,
        //        Currency = chargeRequest.Currency,
        //        SourceTokenOrExistingSourceId = chargeRequest.SourceId,
        //        CustomerId = chargeRequest.CustomerId
        //    });
        //    return charge;
        //}

        //public StripeSource GetSource(string sourceId)
        //{
        //    var source = new StripeSourceService(_settings.StripePrivateKey).Get(sourceId);
        //    return source;
        //}

        //public StripeCustomer ChangeCustomer(string stripeCustomerId, string sourcetoken)
        //{
        //    var customer = new StripeCustomerService(_settings.StripePrivateKey).Update(stripeCustomerId, new StripeCustomerUpdateOptions
        //    {
        //        SourceToken = sourcetoken
        //    });
        //    return customer;
        //}

        //public StripeCustomer CreateCustomer(string email, string description)
        //{
        //    var customer = new StripeCustomerService(_settings.StripePrivateKey).Create(new StripeCustomerCreateOptions
        //    {
        //        Email = email,
        //        Description = description
        //    });
        //    return customer;
        //}

        //public IEnumerable<StripeCustomer> GetCustomers(string stripeCustomerId = null)
        //{
        //    IEnumerable<StripeCustomer> customers = new StripeCustomerService(_settings.StripePrivateKey).List();
        //    return stripeCustomerId != null
        //        ? customers.Where(x => x.Id == stripeCustomerId)
        //        : customers;
        //}

        //public IEnumerable<StripeSubscription> GetSubscriptions()
        //{
        //    IEnumerable<StripeSubscription> subscription = new StripeSubscriptionService(_settings.StripePrivateKey).List();
        //    return subscription;
        //}
    }

    //public class ChargeRequest
    //{
    //    public string CustomerId { get; set; }
    //    public string SourceId { get; set; }
    //    public int Amount { get; set; }
    //    public string Currency { get; set; }
    //}
}
