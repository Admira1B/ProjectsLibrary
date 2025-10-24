namespace ProjectsLibrary.Domain.Models.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<Project> Projects { get; set; } = [];
    }
}
