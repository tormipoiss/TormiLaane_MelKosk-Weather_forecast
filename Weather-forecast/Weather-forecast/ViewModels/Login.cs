using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.ViewModels
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember login?")]
        public bool RememberMe { get; set; }
    }
}
