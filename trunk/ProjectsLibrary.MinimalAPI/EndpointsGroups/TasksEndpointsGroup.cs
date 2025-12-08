using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MinimalAPI.Helpers;
using ProjectsLibrary.MinimalAPI.Interfaces;

namespace ProjectsLibrary.MinimalAPI.EndpointsGroups {
    public class TasksEndpointsGroup : IEndpointsGroup {
        public void MapEndpoints(IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/tasks")
                .WithTags("Tasks")
                .RequireAuthorization(PolicyLevelName.BaseLevel);

            group.MapGet("/", Get)
                .Produces<PagedResult<TaskReadDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:int}", GetById)
                .Produces<TaskReadDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", Add)
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:int}", Update)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id:int}", Delete)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Get(
            [FromServices] IMapper mapper,
            [FromServices] ITaskService service,
            [AsParameters] GetPagedModel model) {
            var builtParams = EndpointHelper.BuildGetMethodModelParams(model);

            var tasksPaged = await service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var tasksDtos = mapper.Map<List<TaskReadDto>>(tasksPaged.Datas);

            var result = new PagedResult<TaskReadDto>() {
                Datas = tasksDtos,
                FilteredRecords = tasksPaged.FilteredRecords,
                TotalRecords = tasksPaged.TotalRecords
            };

            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(
            [FromRoute] int id,
            [FromServices] IMapper mapper,
            [FromServices] ITaskService service) {
            var task = await service.GetByIdNoTrackingAsync(id);

            if (task == null) {
                return Results.NotFound();
            }

            var result = mapper.Map<TaskReadDto>(task);

            return Results.Ok(result);
        }

        private static async Task<IResult> Add(
            [FromServices] IMapper mapper,
            [FromServices] ITaskService service,
            [FromBody] TaskAddDto taskDto) {
            var task = mapper.Map<TaskPL>(taskDto);
            
            await service.AddAsync(task);
            
            return Results.CreatedAtRoute(nameof(GetById), new { id = task.Id }, mapper.Map<TaskReadDto>(task));
        }

        private static async Task<IResult> Update(
            [FromRoute] int id,
            [FromServices] IMapper mapper,
            [FromServices] ITaskService service,
            [FromBody] TaskUpdateDto taskDto) {
            var task = mapper.Map<TaskPL>(taskDto);
            task.Id = id;

            await service.UpdateAsync(task);

            return Results.NoContent();
        }

        private static async Task<IResult> Delete(
            [FromRoute] int id,
            [FromServices] ITaskService service) {
            await service.DeleteAsync(id);

            return Results.NoContent();
        }
    }
}
