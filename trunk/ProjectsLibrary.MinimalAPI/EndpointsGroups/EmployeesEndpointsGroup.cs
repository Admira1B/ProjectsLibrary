using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MinimalAPI.Helpers;
using ProjectsLibrary.MinimalAPI.Interfaces;

namespace ProjectsLibrary.MinimalAPI.EndpointsGroups {
    public class EmployeesEndpointsGroup : IEndpointsGroup {
        public void MapEndpoints(IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/employees")
                .WithTags("Employees");

            group.MapPost("/login", Login)
                .AllowAnonymous();

            group.MapPost("/registration", Register)
                .AllowAnonymous();

            group.MapGet("/", Get)
                .Produces<PagedResult<EmployeeReadDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .RequireAuthorization(PolicyLevelName.ManagmentLevel);

            group.MapGet("/{id:int}", GetById)
                .Produces<EmployeeReadDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(PolicyLevelName.ManagmentLevel);

            group.MapPost("/", Add)
                .Produces(StatusCodes.Status201Created)
                .RequireAuthorization(PolicyLevelName.SupervisorLevel);

            group.MapPut("/{id:int}", Update)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(PolicyLevelName.SupervisorLevel);

            group.MapDelete("/{id:int}", Delete)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(PolicyLevelName.SupervisorLevel);

            group.MapPatch("/{employeeId:int}/assign/{taskId:int}", AssignTaskToEmployee)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(PolicyLevelName.ManagmentLevel);

            group.MapPatch("/{employeeId:int}/unassign/{taskId:int}", UnassignTaskFromEmployee)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(PolicyLevelName.ManagmentLevel);
        }

        private static async Task<IResult> Login(
            [FromServices] IEmployeeService service,
            [FromBody] EmployeeLoginDto employeeDto,
            HttpContext context) {
            var token = await service.LoginAsync(employeeDto.Email, employeeDto.Password);

            AppendTokenToCookies(context, token);

            return Results.NoContent();
        }

        private static async Task<IResult> Register(
            [FromServices] IEmployeeService service,
            [FromServices] IMapper mapper,
            [FromBody] EmployeeAddDto employeeDto) {
            var employee = mapper.Map<Employee>(employeeDto);

            await service.RegisterAsync(employee, employeeDto.Password);

            return Results.NoContent();
        }

        private static async Task<IResult> Get(
            [FromServices] IEmployeeService service,
            [FromServices] IMapper mapper,
            [AsParameters] GetPagedModel model) {
            var builtParams = EndpointHelper.BuildGetMethodModelParams(model);

            var employeesPaged = await service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var employeesDtos = mapper.Map<List<EmployeeReadDto>>(employeesPaged.Datas);

            var result = new PagedResult<EmployeeReadDto>() {
                Datas = employeesDtos,
                FilteredRecords = employeesPaged.FilteredRecords,
                TotalRecords = employeesPaged.TotalRecords
            };

            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(
            [FromRoute] int id,
            [FromServices] IEmployeeService service,
            [FromServices] IMapper mapper) {
            var employee = await service.GetByIdNoTrackingAsync(id);

            if (employee == null) {
                return Results.NotFound();
            }

            var result = mapper.Map<EmployeeReadDto>(employee);

            return Results.Ok(result);
        }

        private static async Task<IResult> Add(
            [FromServices] IEmployeeService service,
            [FromServices] IMapper mapper,
            [FromBody] EmployeeAddDto employeeDto) {
            var employee = mapper.Map<Employee>(employeeDto);

            await service.AddAsync(employee);

            return Results.CreatedAtRoute(nameof(GetById), new { id = employee.Id }, mapper.Map<EmployeeReadDto>(employee));
        }

        private static async Task<IResult> Update(
            [FromRoute] int id,
            [FromServices] IEmployeeService service,
            [FromServices] IMapper mapper,
            [FromBody] EmployeeUpdateDto employeeDto) {
            var employee = mapper.Map<Employee>(employeeDto);
            employee.Id = id;

            await service.UpdateAsync(employee);

            return Results.NoContent();
        }

        private static async Task<IResult> Delete(
            [FromRoute] int id,
            [FromServices] IEmployeeService service) {
            await service.DeleteAsync(id);

            return Results.NoContent();
        }

        private static async Task<IResult> AssignTaskToEmployee(
            [FromRoute] int employeeId,
            [FromRoute] int taskId,
            [FromServices] IEmployeeService service) {
            await service.AssignTaskToEmployee(employeeId, taskId);

            return Results.NoContent();
        }

        private static async Task<IResult> UnassignTaskFromEmployee(
            [FromRoute] int employeeId,
            [FromRoute] int taskId,
            [FromServices] IEmployeeService service) {
            await service.UnassignTaskToEmployee(employeeId, taskId);

            return Results.NoContent();
        }

        private static void AppendTokenToCookies(HttpContext context, string token) {
            context.Response.Cookies.Append("auth-t", token, new CookieOptions {
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
            });
        }
    }
}
