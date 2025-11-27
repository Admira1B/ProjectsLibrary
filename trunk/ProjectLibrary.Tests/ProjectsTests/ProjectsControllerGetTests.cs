using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerGetTests : ProjectsControllerTests {
        [Fact]
        public async Task Get_WithValidModel_ReturnsJsonResult() {
            var model = new GetPagedModel {
                Draw = 1,
                Start = 0,
                Length = 10,
                SearchingValue = "test",
                SearchableFieldsNames = ["id", "name"],
                SortColumn = "id",
                SortDirection = "asc"
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

            var user = MockClaimHelper.BuildManagerClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _projectService.Setup(s => s.GetPaginatedAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<SortParams>(),
                It.IsAny<PageParams>(),
                It.IsAny<int>()))
                .ReturnsAsync(pagedResult);

            _mapper.Setup(m => m.Map<List<ProjectReadDto>>(projects))
                   .Returns(projectsDtos);

            _mapper.Setup(m => m.Map<ProjectReadDto>(It.IsAny<Project>()))
                   .Returns<Project>(p => new ProjectReadDto { Id = p.Id, Name = p.Name });

            var result = await _controller.Get(model);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsProjectReadDto() {
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

            var result = await _controller.GetById(projectId);

            Assert.NotNull(result);
            Assert.IsType<ProjectReadDto>(result);
            Assert.Equal(projectId, result.Id);

            _projectService.Verify(s => s.GetByIdNoTrackingAsync(projectId), Times.Once);
            _mapper.Verify(m => m.Map<ProjectReadDto>(project), Times.Once);
        }

        [Fact]
        public async Task GetProjectWithTasks_WithValidId_ReturnsProjectWithTasks() {
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

            var result = await _controller.GetProjectWithTasks(projectId);

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

    }
}
