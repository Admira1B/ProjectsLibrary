using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.MVC.Models.Project;
using System.Security.Claims;

namespace ProjectsLibrary.MVC.ViewModelBuilders.Interfaces {
    public interface IProjectViewModelBuilder {
        Task<AddProjectViewModel> BuildAddViewModelAsync(ProjectAddDto? projectDto = null);
        Task<DetailsProjectViewModel?> BuildDetailsViewModelAsync(int id, ProjectUpdateDto? projectDto = null);
        Task<IndexProjectViewModel> BuildIndexViewModelAsync(ClaimsPrincipal user);
    }
}