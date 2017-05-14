using Demo.Payment.Adyen;
using Xunit;

namespace Demo.Payment.Adyen.Test
{
    public class HmacHelperTests
    {
        [Fact]
        public void TestCalculateHmac()
        {
            string message = "amount=100&currency=EUR";
            string expectedHex = "b436e3e86cb3800b3864aeecc8d06c126f005e7645803461717a8e4b2de3a905";

            // Test out the HMAC hash method
            string key = "57617b5d2349434b34734345635073433835777e2d244c31715535255a366773755a4d70532a5879793238235f707c4f7865753f3f446e633a21575643303f66";
            var hmacHelper = new HmacHelper();

            string hashHMACHex = hmacHelper.CalculateHashHMACHex(key, message);

            // Assert
            Assert.Equal(expectedHex, hashHMACHex);
        }
    }
}
