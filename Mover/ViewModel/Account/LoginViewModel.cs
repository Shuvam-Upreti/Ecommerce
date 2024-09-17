using System.ComponentModel.DataAnnotations;

namespace Mover.ViewModel.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Obsolete]
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
