using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.MinimalAPI.Helpers;
using ProjectsLibrary.MinimalAPI.Interfaces;

namespace ProjectsLibrary.MinimalAPI.EndpointsGroups {
    public class ProjectsEndpointsGroup : IEndpointsGroup {
        public void MapEndpoints(IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/projects")
                .WithTags("Projects")
                .RequireAuthorization(PolicyLevelName.ManagmentLevel);

            group.MapGet("/", Get)
                .Produces<PagedResult<ProjectReadDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);
            
            group.MapGet("/{id:int}", GetById)
                .Produces<ProjectReadDto>(StatusCodes.Status200OK)
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

            group.MapPatch("/{projectId:int}/add/{employeeId:int}", AddEmployeeToProject)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPatch("/{projectId:int}/remove/{employeeId:int}", RemoveEmployeeFromProject)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Get(
            [FromServices] IProjectService service,
            [FromServices] IMapper mapper,
            [AsParameters] GetPagedModel model) {
            var builtParams = EndpointHelper.BuildGetMethodModelParams(model);

            var projectsPaged = await service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var projectsDtos = mapper.Map<List<ProjectReadDto>>(projectsPaged.Datas);

            var result = new PagedResult<ProjectReadDto>() {
                Datas = projectsDtos,
                FilteredRecords = projectsPaged.FilteredRecords,
                TotalRecords = projectsPaged.TotalRecords
            };

            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(
            [FromRoute] int id,
            [FromServices] IProjectService service,
            [FromServices] IMapper mapper) {
            var project = await service.GetByIdNoTrackingAsync(id);

            if (project == null) {
                return Results.NotFound();
            }

            var result = mapper.Map<ProjectReadDto>(project);

            return Results.Ok(result);
        }

        private static async Task<IResult> Add(
            [FromServices] IProjectService service,
            [FromServices] IMapper mapper,
            [FromBody] ProjectAddDto projectDto) {
            var project = mapper.Map<Project>(projectDto);
            
            await service.AddAsync(project);
            
            return Results.CreatedAtRoute(nameof(GetById), new { id = project.Id }, mapper.Map<ProjectReadDto>(project));
        }

        private static async Task<IResult> Update(
            [FromRoute] int id,
            [FromServices] IProjectService service,
            [FromServices] IMapper mapper,
            [FromBody] ProjectUpdateDto projectDto) {
            var project = mapper.Map<Project>(projectDto);
            project.Id = id;

            await service.UpdateAsync(project);

            return Results.NoContent();
        }

        private static async Task<IResult> Delete(
            [FromRoute] int id,
            [FromServices] IProjectService service) {
            await service.DeleteAsync(id);

            return Results.NoContent();
        }

        private static async Task<IResult> AddEmployeeToProject(
            [FromRoute] int projectId,
            [FromRoute] int employeeId,
            [FromServices] IProjectService service) {
            await service.AddEmployeeToProject(projectId, employeeId);
            
            return Results.NoContent();
        }

        private static async Task<IResult> RemoveEmployeeFromProject(
            [FromRoute] int projectId,
            [FromRoute] int employeeId,
            [FromServices] IProjectService service) {
            await service.RemoveEmployeeFromProject(projectId, employeeId);
            
            return Results.NoContent();
        }
    }
}
