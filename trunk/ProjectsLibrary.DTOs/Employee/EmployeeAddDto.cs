using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectsLibrary.DTOs.Employee
{
    public class EmployeeAddDto
    {
        [Required(ErrorMessage = "Employee`s first name field is required.")]
        [StringLength(40, MinimumLength = 2)]
        public required string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Employee`s last name field is required.")]
        [StringLength(40, MinimumLength = 2)]
        public required string LastName { get; set; } = string.Empty;
        [EmailAddress]
        [Required(ErrorMessage = "Employee`s email field is required.")]
        [StringLength(40, MinimumLength = 7)]
        public required string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Employee`s password field is required.")]
        [PasswordPropertyText]
        [StringLength(250, MinimumLength = 8)]
        public required string Password { get; set; } = string.Empty;
    }
}
