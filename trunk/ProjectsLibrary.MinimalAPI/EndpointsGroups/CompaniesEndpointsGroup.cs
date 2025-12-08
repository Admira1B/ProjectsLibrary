using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.MinimalAPI.Helpers;
using ProjectsLibrary.MinimalAPI.Interfaces;

namespace ProjectsLibrary.MinimalAPI.EndpointsGroups {
    public class CompaniesEndpointsGroup : IEndpointsGroup {
        public void MapEndpoints(IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/companies")
                .WithTags("Companies")
                .RequireAuthorization(PolicyLevelName.SupervisorLevel);

            group.MapGet("/", Get)
                .Produces<PagedResult<CompanyReadDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:int}", GetById)
                .Produces<CompanyReadDto>(StatusCodes.Status200OK)
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
            [FromServices] ICompanyService service,
            [AsParameters] GetPagedModel model) {
            var builtParams = EndpointHelper.BuildGetMethodModelParams(model);

            var companiesPaged = await service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var companiesDtos = mapper.Map<List<CompanyReadDto>>(companiesPaged.Datas);

            var result = new PagedResult<CompanyReadDto>() {
                Datas = companiesDtos,
                FilteredRecords = companiesPaged.FilteredRecords,
                TotalRecords = companiesPaged.TotalRecords
            };

            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(
            [FromRoute] int id,
            [FromServices] IMapper mapper,
            [FromServices] ICompanyService service) {
            var company = await service.GetByIdNoTrackingAsync(id);

            if (company == null) {
                return Results.NotFound();
            }

            var result = mapper.Map<CompanyReadDto>(company);

            return Results.Ok(result);
        }

        private static async Task<IResult> Add(
            [FromServices] IMapper mapper,
            [FromServices] ICompanyService service,
            [FromBody] CompanyAddDto companyDto) {
            var company = mapper.Map<Company>(companyDto);

            await service.AddAsync(company);

            return Results.CreatedAtRoute(nameof(GetById), new { id = company.Id }, mapper.Map<CompanyReadDto>(company));
        }

        private static async Task<IResult> Update(
            [FromRoute] int id,
            [FromServices] IMapper mapper,
            [FromServices] ICompanyService service,
            [FromBody] CompanyUpdateDto companyDto) {
            var company = mapper.Map<Company>(companyDto);
            company.Id = id;

            await service.UpdateAsync(company);

            return Results.NoContent();
        }

        private static async Task<IResult> Delete(
            [FromRoute] int id,
            [FromServices] ICompanyService service) {
            await service.DeleteAsync(id);

            return Results.NoContent();
        }
    }
}
