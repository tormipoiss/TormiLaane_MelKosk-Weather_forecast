using Weather_forecast.Controllers;
using Weather_forecast.ViewModels;

namespace Weather_forecast.Testing
{
    public class AccountTests : TestBase
    {
        [Fact]
        public async Task Should_ReturnSuccessRegister_WhenReturnView()
        {
            Account accountToRegister = new()
            {   
                Username = Guid.NewGuid().ToString(),
                Password = "Abc-1",
                ConfirmPassword = "Abc-1"
            };

            var result = await Svc<AccountController>.Register(accountToRegister);

            Assert.NotNull(result);
        }
    }
}