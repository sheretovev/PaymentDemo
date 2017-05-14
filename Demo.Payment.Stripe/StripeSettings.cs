namespace Demo.Payment.Stripe
{
    public class StripeSettings
    {
        public string StripePrivateKey { get; set; }
        public string StripePublicKey { get; set; }
        public string SepaStatementDescriptor { get; set; }
    }
}
