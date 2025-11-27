namespace ProjectsLibrary.Domain.Models.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Employee> Employees { get; set; } = [];
        public List<TaskPL> Tasks { get; set; } = [];
        public int ProjectManagerId { get; set; }
        public Employee? ProjectManager { get; set; }
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
