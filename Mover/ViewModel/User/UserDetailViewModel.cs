using System.ComponentModel.DataAnnotations;

namespace Mover.ViewModel.User
{
    public class UserDetailViewModel
    {
        public int Id { get; set; }

        public string? AspUserId { get; set; }
        public string? UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfJoin { get; set; }
        public string? Department { get; set; }
        public string Role { get; set; }
        public List<string>? Roles { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    } 
}
