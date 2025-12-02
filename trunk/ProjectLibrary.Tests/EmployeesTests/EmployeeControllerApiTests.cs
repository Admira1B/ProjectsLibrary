using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;

namespace ProjectLibrary.Tests.EmployeesTests {
    public class EmployeeControllerApiTests : EmployeeControllerTests {
        [Fact]
        public async Task Get_WithValidModel_ReturnsOk() {
            var employees = new List<Employee>();
            var companyDtos = new List<EmployeeReadDto>();

            var model = new GetPagedModel {
                Draw = 1,
                Start = 0,
                Length = 10,
                SearchingValue = "test",
                SearchableFieldsNames = ["id", "firstName", "lastName"],
                SortColumn = "id",
                SortDirection = "asc"
            };

            var pagedResult = new PagedResult<Employee> {
                Datas = employees,
                TotalRecords = 100,
                FilteredRecords = 50
            };

            _employeeService.Setup(s => s.GetPaginatedAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<SortParams>(),
                It.IsAny<PageParams>()))
                .ReturnsAsync(pagedResult);

            _mapper.Setup(m => m.Map<List<EmployeeReadDto>>(employees))
                   .Returns(companyDtos);

            var result = await _apiController.Get(model);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetDataOnlyWithoutWorking_WithValidProjectId_ReturnsOk() {
            var projectId = 1;

            var project = new Project {
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

            var result = await _apiController.GetAvailable(projectId);

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
        public async Task GetEmployeeWithTasks_WithValidId_ReturnsOk() {
            var employeeId = 1;
            var employee = new Employee {
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

            var employeeDto = new EmployeeReadDto {
                Id = employeeId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
            };

            _employeeService.Setup(s => s.GetEmployeeWithTasksNoTrackingAsync(employeeId))
                .ReturnsAsync(employee);
            _mapper.Setup(m => m.Map<EmployeeReadDto>(employee))
                .Returns(employeeDto);

            var result = await _apiController.GetEmployeeWithTasks(employeeId);

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
        public async Task GetEmployeeWithProjects_WithValidId_ReturnsOk() {
            var employeeId = 1;
            var employee = new Employee {
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

            var employeeDto = new EmployeeReadDto {
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

            var result = await _apiController.GetEmployeeWithProjects(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<EmployeeReadDto>(okResult.Value);

            Assert.Equal(employeeId, returnedDto.Id);
            Assert.Equal("Jane", returnedDto.FirstName);
            Assert.Equal("Smith", returnedDto.LastName);
            Assert.NotNull(returnedDto.ManagedProjects);

            _employeeService.Verify(s => s.GetEmployeeWithProjectsNoTrackingAsync(employeeId), Times.Once);
            _mapper.Verify(m => m.Map<EmployeeReadDto>(employee), Times.Once);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent() {
            var id = 1;
            _employeeService.Setup(x => x.DeleteAsync(id))
                   .Returns(Task.CompletedTask);

            var result = await _apiController.Delete(id);

            Assert.IsType<NoContentResult>(result);
            _employeeService.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task AssignTaskToEmployee_WithValidIds_ReturnsNoContent() {
            var employeeId = 1;
            var taskId = 100;

            _employeeService.Setup(s => s.AssignTaskToEmployee(employeeId, taskId))
                .Returns(Task.CompletedTask);

            var result = await _apiController.AssignTaskToEmployee(employeeId, taskId);

            Assert.IsType<NoContentResult>(result);
            _employeeService.Verify(s => s.AssignTaskToEmployee(employeeId, taskId), Times.Once);
        }

        [Fact]
        public async Task UnassignTaskFromEmployee_WithValidIds_ReturnsNoContent() {
            var employeeId = 1;
            var taskId = 100;

            _employeeService.Setup(s => s.UnassignTaskToEmployee(employeeId, taskId))
                .Returns(Task.CompletedTask);

            var result = await _apiController.UnassignTaskFromEmployee(employeeId, taskId);

            Assert.IsType<NoContentResult>(result);
            _employeeService.Verify(s => s.UnassignTaskToEmployee(employeeId, taskId), Times.Once);
        }
    }
}
