using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerApiTests : ProjectsControllerTests {
        [Fact]
        public async Task Get_WithValidModel_ReturnsOk() {
            var model = new GetPagedModel {
                Draw = 1,
                Start = 0,
                Length = 10,
                SearchingValue = "test",
                SearchableFieldsNames = ["id", "name"],
                SortColumn = "id",
                SortDirection = "asc"
            };

            var user = MockClaimHelper.BuildManagerClaim();

            _apiController.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var projects = new List<Project>
            {
                new() { Id = 1, Name = "Test Project 1" },
                new() { Id = 2, Name = "Test Project 2" }
            };

            var pagedResult = new PagedResult<Project> {
                Datas = projects,
                TotalRecords = 100,
                FilteredRecords = 50
            };

            var projectsDtos = new List<ProjectReadDto>
            {
                new() { Id = 1, Name = "Test Project 1" },
                new() { Id = 2, Name = "Test Project 2" }
            };

            _mapper.Setup(m => m.Map<List<ProjectReadDto>>(projects)).Returns(projectsDtos);
            _mapper.Setup(m => m.Map<ProjectReadDto>(It.IsAny<Project>())).Returns<Project>(p => new ProjectReadDto { Id = p.Id, Name = p.Name });

            _projectService.Setup(s => s.GetPaginatedAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<SortParams>(),
                It.IsAny<PageParams>(),
                1))
                .ReturnsAsync(pagedResult);

            var result = await _apiController.Get(model);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOk() {
            var projectId = 1;

            var project = new Project {
                Id = projectId,
                Name = "Test Project",
            };

            var projectDto = new ProjectReadDto {
                Id = projectId,
                Name = "Test Project",
            };

            _projectService.Setup(s => s.GetByIdNoTrackingAsync(projectId))
                .ReturnsAsync(project);
            _mapper.Setup(m => m.Map<ProjectReadDto>(project))
                .Returns(projectDto);

            var result = await _apiController.GetById(projectId);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            _projectService.Verify(s => s.GetByIdNoTrackingAsync(projectId), Times.Once);
            _mapper.Verify(m => m.Map<ProjectReadDto>(project), Times.Once);
        }

        [Fact]
        public async Task GetProjectWithTasks_WithValidId_ReturnsOk() {
            var projectId = 1;

            var project = new Project {
                Id = projectId,
                Name = "Test Project",
                Tasks =
                [
                    new() { Id = 1, Name = "Task 1" },
                    new() { Id = 2, Name = "Task 2" }
                ]
            };

            var projectTasksInfoDto = new ProjectTasksInfoDto {
                Id = projectId,
                Name = "Test Project",
            };

            var taskDtos = new List<TaskReadDto>
            {
                new() { Id = 1, Name = "Task 1" },
                new() { Id = 2, Name = "Task 2" }
            };

            _projectService.Setup(s => s.GetProjectWithTasksNoTrackingAsync(projectId))
                .ReturnsAsync(project);
            _mapper.Setup(m => m.Map<ProjectTasksInfoDto>(project))
                .Returns(projectTasksInfoDto);
            _mapper.Setup(m => m.Map<List<TaskReadDto>>(project.Tasks))
                .Returns(taskDtos);

            var result = await _apiController.GetProjectWithTasks(projectId);

            var actionResult = Assert.IsType<ActionResult<ProjectTasksInfoDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedDto = Assert.IsType<ProjectTasksInfoDto>(okResult.Value);

            Assert.Equal(projectId, returnedDto.Id);
            Assert.Equal("Test Project", returnedDto.Name);
            Assert.NotNull(returnedDto.Tasks);

            _projectService.Verify(s => s.GetProjectWithTasksNoTrackingAsync(projectId), Times.Once);
            _mapper.Verify(m => m.Map<ProjectTasksInfoDto>(project), Times.Once);
            _mapper.Verify(m => m.Map<List<TaskReadDto>>(project.Tasks), Times.Once);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent() {
            var id = 1;
            _projectService.Setup(x => x.DeleteAsync(id))
                   .Returns(Task.CompletedTask);

            var result = await _apiController.Delete(id);

            Assert.IsType<NoContentResult>(result);
            _projectService.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task AddEmployeeToProject_WithValidIds_ReturnsNoContent() {
            var projectId = 1;
            var employeeId = 2;

            _projectService.Setup(x => x.AddEmployeeToProject(projectId, employeeId))
                   .Returns(Task.CompletedTask);

            var result = await _apiController.AddEmployeeToProject(projectId, employeeId);

            Assert.IsType<NoContentResult>(result);
            _projectService.Verify(x => x.AddEmployeeToProject(projectId, employeeId), Times.Once);
        }

        [Fact]
        public async Task RemoveEmployeeFromProject_WithValidIds_ReturnsNoContent() {
            var projectId = 1;
            var employeeId = 2;

            _projectService.Setup(x => x.RemoveEmployeeFromProject(projectId, employeeId))
                   .Returns(Task.CompletedTask);

            var result = await _apiController.RemoveEmployeeFromProject(projectId, employeeId);

            Assert.IsType<NoContentResult>(result);
            _projectService.Verify(x => x.RemoveEmployeeFromProject(projectId, employeeId), Times.Once);
        }
    }
}
