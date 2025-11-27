using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;

namespace ProjectLibrary.Tests.TasksTests
{
    public class TasksControllerGetTests : TasksControllerTests
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
                SearchableFieldsNames = ["id", "name"],
                SortColumn = "id",
                SortDirection = "asc"
            };

            var user = MockClaimHelper.BuildManagerClaim();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var tasks = new List<TaskPL>
            {
                new() { Id = 1, Name = "Test Task 1" },
                new() { Id = 2, Name = "Test Task 2" }
            };

            var pagedResult = new PagedResult<TaskPL>
            {
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
                1))
                .ReturnsAsync(pagedResult);

            _mapper.Setup(m => m.Map<List<TaskReadDto>>(tasks))
                   .Returns(tasksDtos);

            var result = await _controller.Get(model);

            Assert.IsType<JsonResult>(result);
        }
    }
}
