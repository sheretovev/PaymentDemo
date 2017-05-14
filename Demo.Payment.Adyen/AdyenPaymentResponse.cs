using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Payment.Adyen
{
    public class AdyenPaymentResponse
    {
        public string authResult { get; set; }
        public string merchantReference { get; set; }
        public string paymentMethod { get; set; }
        public string merchantSig { get; set; }
        public string pspReference { get; set; }
        public string shopperLocale { get; set; }
        public string skinCode { get; set; }
        public string merchantReturnData { get; set; }
    }
}
