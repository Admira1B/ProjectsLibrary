using ProjectsLibrary.DTOs.Project.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace ProjectsLibrary.DTOs.Project
{
    public class ProjectAddDto
    {
        [Required(ErrorMessage = "Project`s name field is required.")]
        [StringLength(25, MinimumLength = 3)]
        public required string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Priority field is required.")]
        [Range(1, 5)]
        public int Priority { get; set; }
        [Required(ErrorMessage = "End date field is required.")]
        [NotPastDate(ErrorMessage = "End date cannot be earlier than today.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Project manager field is required.")]
        public int ProjectManagerId { get; set; }
        [Required(ErrorMessage = "Company field is required.")]
        public int CompanyId { get; set; }
    }
}
