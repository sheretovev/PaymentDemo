using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Payment.Adyen
{
    public class AdyenSettings
    {
        public string MerchantReference { get; set; }
        public string skinCode { get; set; }
        public string merchantAccount { get; set; }
        public string HmacKey { get; set; }
        public double SessionValidityMinutes { get; set; }
        public double ShipBeforeHours { get; set; }
        
    }
}
