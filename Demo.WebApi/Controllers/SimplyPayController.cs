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
    public class SimplyPayController : Controller
    {   
        private StripeSettings paymentSettings;
        private IHttpContextAccessor httpContextAccessor;

        public SimplyPayController(StripeSettings settings, IHttpContextAccessor httpContextAccessor)
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
            return Json(new FirstPaymentData
            {
                CountryCode = "BE",
                Amount = new Random().Next(100, 200),
                Currency = "eur",
                OwnerName = $"Freddy {new Random(seed).Next(999)} Beek {new Random().Next(99)}",
                Description = "Payment Insurance 21-06-2017 - 24-06-2017"
            });
        }

        [HttpGet]
        public IActionResult Get([FromQuery]StripePaymentResponse request)
        {
            var source = new StripeSourceService(stripeSettings.StripePrivateKey).Get(request.source);
            var status = null;
            if(source.Status != "chargeable")
            {
                // remove custom;
                var customer = new StripeSourceService(stripeSettings.StripePrivateKey).Delete(request.source);
                status =  source.Status;
            }
            else{

            var charge = new StripeChargeService(paymentSettings.StripePrivateKey).Create(
                    new StripeChargeCreateOptions
                    {
                        Amount = request.amount,
                        Currency = request.currency,
                        CustomerId = request.customerid,
                        SourceTokenOrExistingSourceId = request.source
                    });
                status = charge.Status;
            }
            return Redirect("http://google.com?status=", status);
        }

        [HttpPut]        
        public IActionResult Put([FromBody]FirstPaymentData request = null)
        {
            var paymentProvider = new StripePaymentProvider(paymentSettings);
            var firstPaymentSource = PaymentSourceCreator.Build(request).Create(paymentSettings, request);
                        
            return firstPaymentSource.Redirect.Url;
        }
    }
}