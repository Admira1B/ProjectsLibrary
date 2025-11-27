namespace ProjectsLibrary.DTOs.Project {
    public class ProjectShortDto {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProjectManagerId { get; set; }
        public int CompanyId { get; set; }
    }
}
