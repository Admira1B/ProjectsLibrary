using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.MVC.Models.Project;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerAddTests : ProjectsControllerTests {
        [Fact]
        public async Task Add_WithValidProject_RedirectsToIndex() {
            var model = new AddProjectViewModel() {
                Project = new ProjectAddDto { Name = "New Project" }
            };

            var project = new Project();

            _mapper.Setup(x => x.Map<Project>(model.Project)).Returns(project);
            _projectService.Setup(x => x.AddAsync(project)).Returns(Task.CompletedTask);

            var result = await _controller.Add(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _mapper.Verify(x => x.Map<Project>(model.Project), Times.Once);
            _projectService.Verify(x => x.AddAsync(project), Times.Once);
        }
    }
}
