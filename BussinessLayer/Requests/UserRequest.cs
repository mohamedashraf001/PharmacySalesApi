using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Requests
{
    public class UserRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot be more than 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }

        [MaxLength(100, ErrorMessage = "Address cannot be more than 100 characters")]
        public string? Address { get; set; }

        [MaxLength(20, ErrorMessage = "National ID cannot be more than 20 characters")]
        public string? NationalId { get; set; }
    }
}
