using Pims.Core.Services;
using Xunit;

namespace Pims.Tests
{
    public class AppInfoServiceTests
    {
        [Fact]
        public void GetAppDisplayName_ReturnsProductName()
        {
            var service = new AppInfoService();

            var result = service.GetAppDisplayName();

            Assert.Equal("Pims", result);
        }
    }
}
