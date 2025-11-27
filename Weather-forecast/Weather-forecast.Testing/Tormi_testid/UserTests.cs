using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Weather_forecast.Controllers;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Weather_forecast.ViewModels;

namespace Weather_forecast.Testing.Tormi_testid
{
    public class UserTests : TestBase
    {
        [Fact]
        public async void Should_ReturnUser_WhenGettingUserByUserIDWithExistingUser()
        {
            Guid userGuid = Guid.NewGuid();

            var db = Svc<DatabaseContext>();
            db.Users.Add(new ApplicationUser { Id = userGuid.ToString() });
            db.SaveChanges();

            var result = await Svc<UserService>().GetUserByID(userGuid);

            Assert.Equal(userGuid.ToString(), result.Id);
        }

        [Fact]
        public async void Should_ReturnNull_WhenGettingUserByUserIDWithNoUsers()
        {
            Guid userGuid = Guid.NewGuid();

            var result = await Svc<UserService>().GetUserByID(userGuid);

            Assert.Null(result);
        }
    }
}