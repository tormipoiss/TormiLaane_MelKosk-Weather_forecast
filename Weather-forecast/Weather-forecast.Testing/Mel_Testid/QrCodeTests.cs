using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather_forecast.Models;
using Weather_forecast.Services;

namespace Weather_forecast.Testing.Mel_Testid
{
    public class QrCodeTests : TestBase
    {
        [Fact]
        public async void ShouldBe_Correct_B64_FromString()
        {
            var result = Svc<QrCodeService>().CreateSharedForecastQRCodeAsB64("https://localhost:5001/Home/Shared?city=Tallinn&shareToken=d2754d3a-ee52-411b-9253-68d451dbf52e&uid=54f30ca8-71c8-4f9f-888e-7d0fbd761055&metric=True&foreCastDate=26.11.2025 11:11:11");
            Assert.StartsWith("iVBORw0KGgoAAAANSUhEUgAABbQAAAW0AQAAAAA22bh6AAANB", result);
            Assert.EndsWith("VfvfkRsaDOgAAAABJRU5ErkJggg==", result);
        }
        [Fact]
        public async void ShouldBe_Correct_B64_FromDto()
        {
            ShareDto shareDto = new ShareDto()
            {
                City = "Tallinn",
                ShareToken = Guid.Parse("d2754d3a-ee52-411b-9253-68d451dbf52e"),
                UID = Guid.Parse("54f30ca8-71c8-4f9f-888e-7d0fbd761055"),
                Date = DateTime.Parse("26.11.2025 11:11:11"),
                Metric = true
            };

            var result = Svc<QrCodeService>().CreateSharedForecastQRCodeAsB64(shareDto);
            Assert.StartsWith("iVBORw0KGgoAAAANSUhEUgAABbQAAAW0AQAAAAA22bh6AAANB", result);
            Assert.EndsWith("VfvfkRsaDOgAAAABJRU5ErkJggg==", result);
        }
    }
}
