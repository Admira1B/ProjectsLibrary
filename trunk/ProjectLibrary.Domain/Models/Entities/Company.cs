namespace ProjectsLibrary.Domain.Models.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Project> Projects { get; set; } = [];
    }
}
