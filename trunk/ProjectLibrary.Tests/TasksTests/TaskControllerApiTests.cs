using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Task;

namespace ProjectLibrary.Tests.TasksTests {
    public class TaskControllerApiTests : TasksControllerTests {
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

            _apiController.ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var tasks = new List<TaskPL>
            {
                new() { Id = 1, Name = "Test Task 1" },
                new() { Id = 2, Name = "Test Task 2" }
            };

            var pagedResult = new PagedResult<TaskPL> {
                Datas = tasks,
                TotalRecords = 100,
                FilteredRecords = 50
            };

            var tasksDtos = new List<TaskReadDto>
            {
                new() { Id = 1, Name = "Test Company 1" },
                new() { Id = 2, Name = "Test Company 2" }
            };

            _taskService.Setup(s => s.GetPaginatedAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<SortParams>(),
                It.IsAny<PageParams>(),
                It.IsAny<int>()))
                .ReturnsAsync(pagedResult);

            _mapper.Setup(m => m.Map<List<TaskReadDto>>(tasks)).Returns(tasksDtos);

            var result = await _apiController.Get(model);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent() {
            var id = 1;
            _taskService.Setup(x => x.DeleteAsync(id))
                   .Returns(Task.CompletedTask);

            var result = await _apiController.Delete(id);

            Assert.IsType<NoContentResult>(result);
            _taskService.Verify(x => x.DeleteAsync(id), Times.Once);
        }
    }
}
