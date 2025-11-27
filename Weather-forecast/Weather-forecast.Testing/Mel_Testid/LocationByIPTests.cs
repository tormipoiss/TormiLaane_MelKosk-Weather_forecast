using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather_forecast.Services;

namespace Weather_forecast.Testing.Mel_Testid
{
    public class LocationByIPTests : TestBase
    {
        [Fact]
        public async void ShouldBeTallinnFromResponse()
        {
            var ipService = Svc<LocationByIPService>();
            string location = await ipService.LocationByIP();
            Assert.Equal("Tallinn",location);
        }
    }
}
