using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Helpers;

namespace ProjectsLibrary.MVC.Controllers.Api {
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(ITaskService service, IMapper mapper) : ControllerBase {
        private readonly ITaskService _service = service;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Get([FromQuery] GetPagedModel model) {
            var builtParams = ControllerHelper.BuildGetMethodModelParams(model);

            int? userId = null;
            var userRole = UserHelper.GetUserRole(User);

            if (userRole <= EmployeeRole.Manager) {
                userId = UserHelper.GetUserId(User);
            }

            var tasksPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams,
                employeeId: userId);

            var tasksDtos = _mapper.Map<List<TaskReadDto>>(tasksPaged.Datas);

            var result = new PagedResult<TaskReadDto>() {
                Datas = tasksDtos,
                FilteredRecords = tasksPaged.FilteredRecords,
                TotalRecords = tasksPaged.TotalRecords
            };

            return Ok
            (new {
                model.Draw,
                recordsFiltered = result.FilteredRecords,
                recordsTotal = result.TotalRecords,
                data = result.Datas
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Delete(int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
