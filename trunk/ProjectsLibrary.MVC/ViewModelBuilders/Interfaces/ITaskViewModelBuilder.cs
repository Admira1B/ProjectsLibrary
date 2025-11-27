using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Models.Task;
using System.Security.Claims;

namespace ProjectsLibrary.MVC.ViewModelBuilders.Interfaces {
    public interface ITaskViewModelBuilder {
        Task<AddTaskViewModel> BuildAddViewModelAsync(ClaimsPrincipal user, int? id = null, TaskAddDto? taskDto = null);
        Task<DetailsTaskViewModel?> BuildDetailsViewModelAsync(int id, ClaimsPrincipal user, TaskUpdateDto? taskDto = null);
        Task<IndexTaskViewModel> BuildIndexViewModelAsync(ClaimsPrincipal user);
    }
}