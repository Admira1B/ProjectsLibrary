using System.ComponentModel.DataAnnotations;
using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.DTOs.Task {
    public class TaskAddDto {
        [Required(ErrorMessage = "Task field is required.")]
        [StringLength(60, MinimumLength = 5)]
        public required string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Status field is required.")]
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
