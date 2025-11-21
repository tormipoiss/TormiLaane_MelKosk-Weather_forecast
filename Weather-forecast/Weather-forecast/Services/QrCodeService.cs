using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class QrCodeService
    {
        public QrCodeService()
        {
        }
        public string CreateSharedForecastQRCodeAsB64(string url)
        {
            return QrAsB64(url);
        }
        public string CreateSharedForecastQRCodeAsB64(ShareDto dto)
        {
            return QrAsB64($"https://localhost:5001/Home/Shared?city={dto.City}&shareToken={dto.ShareToken}&uid={dto.UID}&metric={dto.Metric}&foreCastDate={dto.Date}");
        }
        private string QrAsB64(string data)
        {
            var qrGen = new QRCodeGenerator();
            var imgType = Base64QRCode.ImageType.Png;
            QRCodeData qrCodeData = qrGen.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20, Color.Black, Color.White, true, imgType);
            return qrCodeImageAsBase64;
        }
    }
}
