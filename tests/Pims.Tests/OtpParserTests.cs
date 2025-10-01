using Pims.Core.Utilities;
using Xunit;

namespace Pims.Tests
{
    public class OtpParserTests
    {
        [Theory]
        [InlineData("Your OTP is 123456", "123456")]
        [InlineData("Code: 987654 expires soon", "987654")]
        [InlineData("[OTP] 000123", "000123")]
        public void TryExtract_ReturnsTrue_WhenDigitsPresent(string subject, string expected)
        {
            var success = OtpParser.TryExtract(subject, out var otp);

            Assert.True(success);
            Assert.Equal(expected, otp);
        }

        [Fact]
        public void TryExtract_ReturnsFalse_WhenMissingDigits()
        {
            var success = OtpParser.TryExtract("No code available", out var otp);

            Assert.False(success);
            Assert.Equal(string.Empty, otp);
        }
    }
}
