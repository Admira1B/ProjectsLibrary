using ProjectsLibrary.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectsLibrary.DTOs.Task
{
    public class TaskUpdateDto
    {
        [Required(ErrorMessage = "Task field is required.")]
        [StringLength(60, MinimumLength = 5)]
        public required string Name { get; set; }
        public TaskPLStatus Status { get; set; }
        [StringLength(350)]
        public string? Comment { get; set; }
        [Required(ErrorMessage = "Project field is required.")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Creator field is required.")]
        public int CreatorId { get; set; }
        [Required(ErrorMessage = "Executor field is required.")]
        public int ExecutorId { get; set; }
    }
}
