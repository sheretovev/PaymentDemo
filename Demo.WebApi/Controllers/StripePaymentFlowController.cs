using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Demo.Payment.Adyen;
using Demo.Payment;
using Demo.Payment.Stripe;
using Stripe;
using Microsoft.AspNetCore.Http;

namespace Demo.WebApi.Controllers
{
    [Route("[controller]")]
    public class StripePaymentFlowController : Controller
    {   
        private StripeSettings paymentSettings;
        private IHttpContextAccessor httpContextAccessor;

        public StripePaymentFlowController(StripeSettings settings, IHttpContextAccessor httpContextAccessor)
        {
            this.paymentSettings = settings;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Example endpoint which generates test data to make a payment PUT request
        /// </summary>
        /// <param name="seed">Random number</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{seed}")]
        public IActionResult Get([FromRoute]int seed)
        {
            return Json(new StripePaymentRequest
            {
                CountryCode = "BE",
                Email = $"Demo.test.{new Random(seed).Next(999)}@mailinator.com",
                AmountCents = new Random().Next(100, 200),
                Currency = "eur",
                Name = $"Freddy {new Random(seed).Next(999)} Beek {new Random().Next(99)}",
                Type = StripeSourceType.Ideal,
                ReturnUrl = "http://quotesideas.com/wp-content/uploads/2015/02/Event-Happy-Easter-Big-Sunday-Wallpaper.jpg",
                ReturnUrlParams = new Dictionary<string, string> {
                    { "CarTypeId", "2342"},
                    { "ColorId", "2"},
                    { "CaseId", "35"}
                },
                Phone = $"064{new Random(99999999)}",
                PostalCode = $"{new Random(9999)}AB",
                Line1 = $"Street {new Random(9999)}",
                CityOrTown = $"City {new Random(293845)}",
                Iban = "DE89370400440532013000"
            });
        }

        [HttpGet]
        public IActionResult Get([FromQuery]StripePaymentResponse request)
        {
            var charge = new StripeChargeService(paymentSettings.StripePrivateKey).Create(
                    new StripeChargeCreateOptions
                    {
                        Amount = request.amount,
                        Currency = request.currency,
                        CustomerId = request.customerid,
                        SourceTokenOrExistingSourceId = request.source
                    });

            return Redirect(request.returnUrl.Replace("_qm_", "?").Replace("_amp_", "&"));// todo: rework this hotfix
        }

        [HttpPut]        
        public IActionResult Put([FromQuery]bool? firstPayment = false, [FromQuery]bool? sepa_debit = false, [FromQuery]bool? customer = false, [FromQuery]bool? subscription = false, [FromBody]StripePaymentRequest request = null)
        {
            var paymentProvider = new StripePaymentProvider(paymentSettings);
            // Create customer if requested
            var customerObject = customer.HasValue && customer.Value
                ? new StripeCustomerService(paymentSettings.StripePrivateKey).Create(new StripeCustomerCreateOptions
                {
                    Email = request.Email,
                    Description = request.Name
                })
                : null;
            // define return url
            request.ReturnUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/{nameof(StripePaymentFlowController).Replace(nameof(Controller), string.Empty)}?returnUrl=" +
                    ($"{request.ReturnUrl}?{string.Concat(request.ReturnUrlParams.Select(x => $"&{x.Key}={x.Value}")).Substring(1)}").Replace("?", "_qm_").Replace("&", "_amp_") +
                    (customerObject != null ? $"&customerId={customerObject.Id}" : "") +
                    $"&amount={request.AmountCents}" +
                    $"&currency={request.Currency}";

            // Create first payment source if requested
            var dataForSource = new FirstPaymentData
            {
                Amount = request.AmountCents,
                Currency = request.Currency,
                OwnerName = request.Name,
                RedirectReturnUrl = request.ReturnUrl,
                CountryCode = request.CountryCode
            };
            var firstPaymentSource = PaymentSourceCreator.Build(dataForSource).Create(paymentSettings, dataForSource);
            // create sepa source if requested
            var sepaSource = sepa_debit.HasValue && sepa_debit.Value ? new StripeSourceService(paymentSettings.StripePrivateKey).Create(new StripeSourceCreateOptions
            {
                Type = StripeSourceType.SepaDebit,
                Amount = request.AmountCents,
                Currency = request.Currency,
                Owner = new StripeSourceOwner
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    PostalCode = request.PostalCode,
                    Line1 = request.Line1,
                    CityOrTown = request.CityOrTown,
                    Country = "NL"
                },
                SepaDebitIban = !string.IsNullOrEmpty(request.Iban) ? request.Iban : "DE89370400440532013000"
            }) : null;

            if (customerObject != null)
            {
                // Assign customer to the source            
                new StripeCustomerService(paymentSettings.StripePrivateKey).Update(customerObject.Id, new StripeCustomerUpdateOptions
                {
                    SourceToken = (sepaSource ?? firstPaymentSource)?.Id
                });
            }
            
            // Create plan and subscriptions to sepaDirectDebit
            if (subscription.HasValue && subscription.Value && customerObject != null)
            {
                var planId = $"1_{request.Currency}";
                var planName = $"One {request.Currency} plan";

                var plan = new StripePaymentProvider(paymentSettings).GetPlan(planId)
                        ?? new StripePlanService(paymentSettings.StripePrivateKey).Create(new StripePlanCreateOptions
                        {
                            Amount = 100,
                            Currency = request.Currency,
                            Interval = StripePlanIntervals.Month,
                            Name = planName,
                            StatementDescriptor = paymentSettings.SepaStatementDescriptor,
                            TrialPeriodDays = 30,
                            IntervalCount = 1,
                            Id = planId
                        });
                new StripeSubscriptionService(paymentSettings.StripePrivateKey).Create(customerObject.Id, new StripeSubscriptionCreateOptions
                {
                    PlanId = planId,
                    Quantity = request.AmountCents
                });
            }            
            return Json((firstPayment.HasValue && firstPayment.Value) ?
                firstPaymentSource.Redirect.Url
                : ($"{request.ReturnUrl}?{string.Concat(request.ReturnUrlParams.Select(x => $"&{x.Key}={x.Value}")).Substring(1)}").Replace("?", "_qm_").Replace("&", "_amp_"));
        }
    }
}