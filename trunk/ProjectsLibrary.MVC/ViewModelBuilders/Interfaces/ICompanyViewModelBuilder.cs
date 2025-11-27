using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.MVC.Models.Company;

namespace ProjectsLibrary.MVC.ViewModelBuilders.Interfaces {
    public interface ICompanyViewModelBuilder {
        Task<DetailsCompanyViewModel?> BuildDetailsViewModelAsync(int id, CompanyUpdateDto? companyDto = null);
        Task<IndexCompanyViewModel> BuildIndexViewModelAsync(CompanyAddDto? companyDto = null);
    }
}