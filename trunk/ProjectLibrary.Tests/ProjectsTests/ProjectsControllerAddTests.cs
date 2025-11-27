using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.DTOs.Project;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerAddTests : ProjectsControllerTests {
        [Fact]
        public async Task Add_WithValidProject_RedirectsToIndex() {
            var projectAddDto = new ProjectAddDto { Name = "New Project" };
            var projectEntity = new Project { Name = "New Project" };

            _mapper.Setup(x => x.Map<Project>(projectAddDto))
                   .Returns(projectEntity);
            _projectService.Setup(x => x.AddAsync(projectEntity))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Add(projectAddDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mapper.Verify(x => x.Map<Project>(projectAddDto), Times.Once);
            _projectService.Verify(x => x.AddAsync(projectEntity), Times.Once);
        }

        [Fact]
        public async Task AddEmployeeToProject_WithValidIds_ReturnsNoContent() {
            var projectId = 1;
            var employeeId = 2;

            _projectService.Setup(x => x.AddEmployeeToProject(projectId, employeeId))
                   .Returns(Task.CompletedTask);

            var result = await _controller.AddEmployeeToProject(projectId, employeeId);

            Assert.IsType<NoContentResult>(result);
            _projectService.Verify(x => x.AddEmployeeToProject(projectId, employeeId), Times.Once);
        }

        [Fact]
        public async Task RemoveEmployeeFromProject_WithValidIds_ReturnsNoContent() {
            var projectId = 1;
            var employeeId = 2;

            _projectService.Setup(x => x.RemoveEmployeeFromProject(projectId, employeeId))
                   .Returns(Task.CompletedTask);

            var result = await _controller.RemoveEmployeeFromProject(projectId, employeeId);

            Assert.IsType<NoContentResult>(result);
            _projectService.Verify(x => x.RemoveEmployeeFromProject(projectId, employeeId), Times.Once);
        }
    }
}
