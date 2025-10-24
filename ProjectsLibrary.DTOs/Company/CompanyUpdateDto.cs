using System.ComponentModel.DataAnnotations;

namespace ProjectsLibrary.DTOs.Company
{
    public class CompanyUpdateDto
    {
        [Required(ErrorMessage = "Company`s name field is required.")]
        [StringLength(25, MinimumLength = 3)]
        public required string Name { get; set; }
    }
}
