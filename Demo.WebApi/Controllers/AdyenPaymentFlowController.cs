using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Demo.Payment.Adyen;

namespace Demo.WebApi.Controllers
{
    /*
         "AdyenSettings": {
            "MerchantReference": "SKINTEST-test",
            "skinCode": "wWhnyIR5",
            "merchantAccount": "LeasePlanNL",
            "HmacKey": "909386E1AE37F0557E2004C146449FE5257A454F5089DAB062135918557B2866",
            "ShipBeforeHours": 48,
            "SessionValidityMinutes": 120
          }, 
    */
    [Route("[controller]")]
    public class AdyenPaymentFlowController : Controller
    {
        private AdyenSettings paymentSettings;
        public AdyenPaymentFlowController(AdyenSettings settings)
        {
            this.paymentSettings = settings;
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
            return Json(new AdyenPaymentRequest()
            {
                Email = $"Demo.test.{new Random(seed).Next(999)}@mailinator.com",
                Amount = new Random().Next(100),
                Currency = "eur",
                Name = $"Freddy {new Random(seed).Next(999)} Beek {new Random().Next(99)}",
                ReturnUrl = "http://privatelease.acceptatie.jungleminds.nl/word-private-leaser/5",
                ReturnUrlParams = new Dictionary<string, string> {
                    { "CarTypeId", "2342"},
                    { "ColorId", "2"},
                    { "CaseId", "35"}
                }
            });
        }

        /// <summary>
        /// Landing Endpoint for Adyen payment flow
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery]AdyenPaymentResponse request)
        {
            // check payment
            var list = new SortedList<string, string>();
            list.Add(nameof(request.shopperLocale), request.shopperLocale);
            list.Add(nameof(request.authResult), request.authResult);
            list.Add(nameof(request.merchantReference), request.merchantReference);
            list.Add(nameof(request.merchantReturnData), request.merchantReturnData);
            list.Add(nameof(request.skinCode), request.skinCode);
            list.Add(nameof(request.paymentMethod), request.paymentMethod);
            list.Add(nameof(request.pspReference), request.pspReference);
            var messageToSign = string.Concat(string.Join(":", list.Keys), ":", string.Join(":", list.Values.Select(x => x.Replace("\\", "\\\\").Replace(":", "\\:"))));

            var sign = new HmacHelper().CalculateHashHMACHex(paymentSettings.HmacKey, messageToSign);

            if (sign == request.merchantSig)
            {
                var returnUrl = request.merchantReturnData.Replace("_qm_", "?").Replace("_amp_", "&");// todo: rework this hotfix
                return Redirect(returnUrl);
            }
            return BadRequest();            
        }

        /// <summary>
        /// Submit Adyen payment request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public string Put([FromBody]AdyenPaymentRequest request)
        {            
            var list = new SortedList<string, string>();
            list.Add("merchantAccount", paymentSettings.merchantAccount);
            list.Add("currencyCode", request.Currency.ToUpperInvariant());
            list.Add("paymentAmount", request.Amount.ToString());
            list.Add("sessionValidity", DateTime.Now.AddMinutes(paymentSettings.SessionValidityMinutes).ToString(AdyenPaymentProvider.AdyenDateFormat));
            list.Add("shipBeforeDate", DateTime.Now.AddHours(paymentSettings.ShipBeforeHours).ToString(AdyenPaymentProvider.AdyenDateFormat));
            list.Add("merchantReference", paymentSettings.MerchantReference);
            list.Add("skinCode", paymentSettings.skinCode);
            list.Add("shopperEmail", request.Email);
            list.Add("merchantReturnData", $"{request.ReturnUrl}?{(request.ReturnUrlParams == null ? string.Empty : string.Concat(request.ReturnUrlParams.Select(x => $"&{x.Key}={x.Value}")).Substring(1).Replace("?", "_qm_").Replace("&", "_amp_"))}");
            return new AdyenPaymentProvider(paymentSettings).BuildLink(list);
        }
    }
}