using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password is not correct! Try again!")]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
