using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Weather_forecast.Models;
using Weather_forecast.Services;

namespace Weather_forecast.Testing.Mel_Testid
{
    public class ShareTests : TestBase
    {
        private async Task CreateShare(CityShare share)
        {
            var srvc = Svc<SharesServices>();
            await srvc.AddAsync(share);

        }
        [Fact]
        public async void ShouldBe_Correct_NumberOf_Shares()
        {
            var srvc = Svc<SharesServices>();
            List<CityShare> shares = new();
            string uid = Guid.NewGuid().ToString();
            for(int i = 0; i < 5; i++)
            {
                var share = new CityShare()
                {
                    ShareToken = Guid.NewGuid().ToString(),
                    UserId = uid,
                    City = Guid.NewGuid().ToString(),
                    ViewCount = i,
                    DayAmount = i,
                    Metric = false,
                    forecastDate = DateTime.Now,
                };
                await CreateShare(share);
                shares.Add(share);
            }
            var createdShares = srvc.GetUserShares(uid);
            Assert.NotEmpty(createdShares);
            Assert.Equal(shares.Count, createdShares.Count);

            foreach(CityShare share in createdShares)
            {
                var gottenShare = await srvc.GetShareByShareToken(Guid.Parse(share.ShareToken));
                Assert.Equal(share.City, gottenShare.City);
            }

        }
        [Fact]
        public async void GetCorrect_ShareFromToken()
        {
            var srvc = Svc<SharesServices>();
            var share = new CityShare()
            {
                ShareToken = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                City = Guid.NewGuid().ToString(),
                ViewCount = 1,
                DayAmount = 1,
                Metric = false,
                forecastDate = DateTime.Now,
            };
            await CreateShare(share);
            var fromService = await srvc.GetShareByShareToken(Guid.Parse(share.ShareToken));
            Assert.Equal(share.UserId, fromService.UserId);
            Assert.Equal(share.City, fromService.City);
        }
        [Fact]
        public async void RemoveCorrect_Share()
        {
            var srvc = Svc<SharesServices>();
            var share = new CityShare()
            {
                ShareToken = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                City = Guid.NewGuid().ToString(),
                ViewCount = 1,
                DayAmount = 1,
                Metric = false,
                forecastDate = DateTime.Now,
            };
            await CreateShare(share);
            srvc.Remove(share);
            var exists = await srvc.GetShareByShareToken(Guid.Parse(share.ShareToken));
            Assert.Equal(default, exists);
        }
        [Fact]
        public async void AddCorrect_Share()
        {
            var srvc = Svc<SharesServices>();
            var share = new CityShare()
            {
                ShareToken = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                City = Guid.NewGuid().ToString(),
                ViewCount = 1,
                DayAmount = 1,
                Metric = false,
                forecastDate = DateTime.Now,
            };
            await CreateShare(share);
            var exists = await srvc.GetShareByShareToken(Guid.Parse(share.ShareToken));
            Assert.NotEqual(default, exists);
            Assert.Equal(share.UserId, exists.UserId);
            Assert.Equal(share.City, exists.City);
        }
        [Fact]
        public async void ShouldBe_Correct_SharedShare()
        {
            var srvc = Svc<SharesServices>();
            string user = Guid.NewGuid().ToString();
            var share = new CityShare()
            {
                ShareToken = Guid.NewGuid().ToString(),
                UserId = user,
                City = "Tallinn",
                ViewCount = 1,
                DayAmount = 1,
                Metric = false,
                forecastDate = DateTime.Now,
            };
            await CreateShare(share);
            var exists = await srvc.GetAlreadySharedShare(user,share.City);
            Assert.NotEqual(default, exists);
            Assert.Equal(share.City,exists.City);
        }
    }
}
