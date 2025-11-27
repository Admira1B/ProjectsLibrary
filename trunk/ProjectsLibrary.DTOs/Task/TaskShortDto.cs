using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.DTOs.Task
{
    public class TaskShortDto
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public TaskPLStatus Status { get; set; }
        public string StatusString => Status.StatusToString();
        public string? Comment { get; set; }
    }
}
