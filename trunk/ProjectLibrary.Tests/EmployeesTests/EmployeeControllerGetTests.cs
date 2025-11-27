using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;

namespace ProjectLibrary.Tests.EmployeesTests
{
    public class EmployeeControllerGetTests : EmployeeControllerTests
    {
        [Fact]
        public async Task Get_WithValidModel_ReturnsJsonResult()
        {
            var model = new GetPagedModel
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SearchingValue = "test",
                SearchableFieldsNames = ["id", "firstName", "lastName"],
                SortColumn = "id",
                SortDirection = "asc"
            };

            var employees = new List<Employee>
            {
                new() { Id = 1, FirstName = "Jhon", LastName = "Doeone", Email = "testemail@test.te" },
                new() { Id = 2, FirstName = "Jhon", LastName = "Doetwo", Email = "testemail@test.te" }
            };

            var pagedResult = new PagedResult<Employee>
            {
                Datas = employees,
                TotalRecords = 100,
                FilteredRecords = 50
            };

            var companyDtos = new List<EmployeeReadDto>
            {
                new() { Id = 1, FirstName = "Jhon", LastName = "Doeone", Email = "testemail@test.te" },
                new() { Id = 2, FirstName = "Jhon", LastName = "Doetwo", Email = "testemail@test.te" }
            };

            _employeeService.Setup(s => s.GetPaginatedAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<SortParams>(),
                It.IsAny<PageParams>()))
                .ReturnsAsync(pagedResult);

            _mapper.Setup(m => m.Map<List<EmployeeReadDto>>(employees))
                   .Returns(companyDtos);

            var result = await _controller.Get(model);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetDataOnlyWithoutWorking_WithValidProjectId_ReturnsFilteredEmployees()
        {
            var projectId = 1;

            var project = new Project
            {
                Id = projectId,
                Name = "Project Name",
                Employees =
                [
                    new() { Id = 1, FirstName = "John", LastName = "Updated", Email = "john@test.com" },
                    new() { Id = 2, FirstName = "Jane", LastName = "Updated", Email = "jane@test.com" }
                ],
                ProjectManagerId = 3
            };

            var allEmployees = new List<Employee>
            {
                new() { Id = 1, FirstName = "John", LastName = "Updated", Email = "john@test.com" },
                new() { Id = 2, FirstName = "Jane", LastName = "Updated", Email = "jane@test.com" },
                new() { Id = 3, FirstName = "Bob", LastName = "Smith", Email = "bob@test.com" },
                new() { Id = 4, FirstName = "Alice", LastName = "Johnson", Email = "alice@test.com" },
                new() { Id = 5, FirstName = "Charlie", LastName = "Brown", Email = "charlie@test.com" }
            };

            var expectedFilteredEmployees = new List<Employee>
            {
                allEmployees[3],
                allEmployees[4]
            };

            var expectedMappedDtos = new List<EmployeeReadDto>
            {
                new() { Id = 4, FirstName = "Alice", LastName = "Johnson", Email = "alice@test.com" },
                new() { Id = 5, FirstName = "Charlie", LastName = "Brown", Email = "charlie@test.com" }
            };

            _projectService.Setup(p => p.GetByIdNoTrackingAsync(projectId))
                .ReturnsAsync(project);
            _employeeService.Setup(s => s.GetDataOnlyAsync())
                .ReturnsAsync(allEmployees);
            _mapper.Setup(m => m.Map<List<EmployeeReadDto>>(expectedFilteredEmployees))
                .Returns(expectedMappedDtos);

            var result = await _controller.GetDataOnlyWithoutWorking(projectId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDtos = Assert.IsType<List<EmployeeReadDto>>(okResult.Value);

            Assert.Equal(2, returnedDtos.Count);
            Assert.Contains(returnedDtos, d => d.Id == 4);
            Assert.Contains(returnedDtos, d => d.Id == 5);
            Assert.DoesNotContain(returnedDtos, d => d.Id == 1 || d.Id == 2 || d.Id == 3);

            _projectService.Verify(p => p.GetByIdNoTrackingAsync(projectId), Times.Once);
            _employeeService.Verify(s => s.GetDataOnlyAsync(), Times.Once);
            _mapper.Verify(m => m.Map<List<EmployeeReadDto>>(expectedFilteredEmployees), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeWithTasks_WithValidId_ReturnsEmployeeWithTasks()
        {
            var employeeId = 1;
            var employee = new Employee
            {
                Id = employeeId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                CreatedTasks =
                [
                    new() { Id = 1, Name = "Task 1" },
                    new() { Id = 2, Name = "Task 2" }
                ]
            };

            var employeeDto = new EmployeeReadDto
            {
                Id = employeeId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
            };

            _employeeService.Setup(s => s.GetEmployeeWithTasksNoTrackingAsync(employeeId))
                .ReturnsAsync(employee);
            _mapper.Setup(m => m.Map<EmployeeReadDto>(employee))
                .Returns(employeeDto);

            var result = await _controller.GetEmployeeWithTasks(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<EmployeeReadDto>(okResult.Value);

            Assert.Equal(employeeId, returnedDto.Id);
            Assert.Equal("John", returnedDto.FirstName);
            Assert.Equal("Doe", returnedDto.LastName);
            Assert.NotNull(returnedDto.CreatedTasks);

            _employeeService.Verify(s => s.GetEmployeeWithTasksNoTrackingAsync(employeeId), Times.Once);
            _mapper.Verify(m => m.Map<EmployeeReadDto>(employee), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeWithProjects_WithValidId_ReturnsEmployeeWithProjects()
        {
            var employeeId = 1;
            var employee = new Employee
            {
                Id = employeeId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                WorkingProjects =
                [
                    new() { Id = 1, Name = "Project A" },
                    new() { Id = 2, Name = "Project B" }
                ]
            };

            var employeeDto = new EmployeeReadDto
            {
                Id = employeeId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                WorkingProjects = new List<ProjectShortDto>
            {
                new() { Id = 1, Name = "Project A" },
                new() { Id = 2, Name = "Project B" }
            }
            };

            _employeeService.Setup(s => s.GetEmployeeWithProjectsNoTrackingAsync(employeeId))
                .ReturnsAsync(employee);
            _mapper.Setup(m => m.Map<EmployeeReadDto>(employee))
                .Returns(employeeDto);

            var result = await _controller.GetEmployeeWithProjects(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<EmployeeReadDto>(okResult.Value);

            Assert.Equal(employeeId, returnedDto.Id);
            Assert.Equal("Jane", returnedDto.FirstName);
            Assert.Equal("Smith", returnedDto.LastName);
            Assert.NotNull(returnedDto.ManagedProjects);

            _employeeService.Verify(s => s.GetEmployeeWithProjectsNoTrackingAsync(employeeId), Times.Once);
            _mapper.Verify(m => m.Map<EmployeeReadDto>(employee), Times.Once);
        }
    }
}