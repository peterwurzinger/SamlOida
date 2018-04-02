using Xunit;

namespace SamlOida.Test
{
    public class SamlOptionsTests
    {
        [Fact]
        public void CreatingOptionsShouldReturnMostSecureOptions()
        {
            var options = new SamlOptions();

            Assert.True(options.AcceptSignedMessagesOnly);
            Assert.True(options.SignOutgoingMessages);
        }
    }
}
