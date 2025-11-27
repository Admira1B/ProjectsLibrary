using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.DTOs.Project;

namespace ProjectsLibrary.MVC.Models.Company {
    public class DetailsCompanyViewModel {
        public int Id { get; set; }
        public CompanyUpdateDto Company { get; set; } = null!;
        public List<ProjectReadDto> Projects { get; set; } = [];
    }
}
