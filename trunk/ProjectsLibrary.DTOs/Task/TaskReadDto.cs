using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;

namespace ProjectsLibrary.DTOs.Task
{
    public class TaskReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public TaskPLStatus Status { get; set; }
        public string StatusString => Status.StatusToString();
        public string? Comment { get; set; }
        public int ProjectId { get; set; }
        public ProjectShortDto? Project { get; set; }
        public int CreatorId { get; set; }
        public EmployeeShortDto? Creator { get; set; }
        public int ExecutorId { get; set; }
        public EmployeeShortDto? Executor { get; set; }
    }
}
