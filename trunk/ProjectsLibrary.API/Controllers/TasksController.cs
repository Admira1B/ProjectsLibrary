using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.API.Extensions;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Task;

namespace ProjectsLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(ITaskService service, IMapper mapper) : ControllerBase
    {
        private readonly ITaskService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<TaskReadDto>>> Get([FromQuery]GetPagedModel model)
        {
            var builtParams = ControllersExtensions.BuildGetMethodModelParams(model);

            var tasksPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var tasksDtos = _mapper.Map<List<TaskReadDto>>(tasksPaged.Datas);

            var result = new PagedResult<TaskReadDto>()
            {
                Datas = tasksDtos,
                FilteredRecords = tasksPaged.FilteredRecords,
                TotalRecords = tasksPaged.TotalRecords
            };

            return Ok(tasksDtos);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskReadDto>> GetById([FromRoute]int id) 
        {
            var task = await _service.GetByIdNoTrackingAsync(id);
            var taskDto = _mapper.Map<TaskReadDto>(task);
            return Ok(taskDto);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody]TaskAddDto taskDto) 
        {
            var task = _mapper.Map<TaskPL>(taskDto);
            await _service.AddAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, _mapper.Map<TaskReadDto>(task));
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update([FromRoute]int id, [FromBody]TaskUpdateDto taskDto) 
        {
            var task = _mapper.Map<TaskPL>(taskDto);
            task.Id = id;
            await _service.UpdateAsync(task);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute]int id) 
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
