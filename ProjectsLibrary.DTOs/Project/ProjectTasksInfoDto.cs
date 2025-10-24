using ProjectsLibrary.DTOs.Task;

namespace ProjectsLibrary.DTOs.Project
{
    public class ProjectTasksInfoDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<TaskReadDto> Tasks { get; set; } = [];
    }
}
