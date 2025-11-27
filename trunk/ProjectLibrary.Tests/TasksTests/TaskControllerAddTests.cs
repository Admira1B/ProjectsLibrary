using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.TasksTests
{
    public class TaskControllerAddTests : TasksControllerTests
    {
        [Fact]
        public async Task Add_WithValidDto_ReturnsRedirectToActionResult()
        {
            var taskAddDto = new TaskAddDto
            {
                Name = "New Task",
            };

            var task = new TaskPL
            {
                Name = "New Task",
            };

            _mapper.Setup(m => m.Map<TaskPL>(taskAddDto))
                   .Returns(task);

            _taskService.Setup(s => s.AddAsync(task))
                          .Returns(Task.CompletedTask);

            var result = await _controller.Add(taskAddDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mapper.Verify(m => m.Map<TaskPL>(taskAddDto), Times.Once);
            _taskService.Verify(s => s.AddAsync(task), Times.Once);
        }
    }
}
