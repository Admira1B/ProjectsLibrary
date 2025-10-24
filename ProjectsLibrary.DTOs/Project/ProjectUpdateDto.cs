using System.ComponentModel.DataAnnotations;

namespace ProjectsLibrary.DTOs.Project
{
    public class ProjectUpdateDto
    {
        [Required(ErrorMessage = "Project`s name field is required.")]
        [StringLength(25, MinimumLength = 3)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Priority field is required.")]
        [Range(1, 5)]
        public int Priority { get; set; }
        [Required(ErrorMessage = "Start date field is required.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date field is required.")]
        [NotEarlierDate("StartDate", ErrorMessage = "End date cannot be earlier than start date.")]
        public DateTime EndDate { get; set; }
        public int ProjectManagerId { get; set; }
        [Required(ErrorMessage = "Company-customer is required.")]
        public int CompanyId { get; set; }
    }
}
