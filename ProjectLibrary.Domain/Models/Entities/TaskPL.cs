using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.Domain.Models.Entities
{
    public class TaskPL
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public TaskPLStatus Status { get; set; }
        public string? Comment { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public int CreatorId { get; set; }
        public Employee? Creator { get; set; }
        public int ExecutorId { get; set; }
        public Employee? Executor { get; set; }
    }
}
