using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectsLibrary.DTOs.Employee
{
    public class EmployeeLoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;
        [Required]
        [PasswordPropertyText]
        [StringLength(250, MinimumLength = 8)]
        public required string Password { get; set; } = string.Empty;
    }
}
