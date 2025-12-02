using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Project;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerDetailsTests : ProjectsControllerTests {
        [Fact]
        public async Task Details_WithValidData_RedirectsToIndex() {
            var model = new DetailsProjectViewModel {
                Id = 1,
                Project = new ProjectUpdateDto { 
                    Name = "Updated Project" 
                }
            };

            var project = new Project();

            _mapper.Setup(x => x.Map<Project>(model.Project)).Returns(project);
            _projectService.Setup(x => x.UpdateAsync(project)).Returns(Task.CompletedTask);

            var result = await _controller.Details(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _mapper.Verify(x => x.Map<Project>(model.Project), Times.Once);
            _projectService.Verify(x => x.UpdateAsync(project), Times.Once);
        }
    }
}
