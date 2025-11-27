using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerUpdateTests : ProjectsControllerTests {
        [Fact]
        public async Task Update_WithValidData_RedirectsToIndex() {
            var id = 1;
            var projectUpdateDto = new ProjectUpdateDto { Name = "Updated Project" };
            var projectEntity = new Project { Id = id, Name = "Updated Project" };

            _mapper.Setup(x => x.Map<Project>(projectUpdateDto))
                   .Returns(projectEntity);
            _projectService.Setup(x => x.UpdateAsync(projectEntity))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Update(id, projectUpdateDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(id, projectEntity.Id);

            _mapper.Verify(x => x.Map<Project>(projectUpdateDto), Times.Once);
            _projectService.Verify(x => x.UpdateAsync(projectEntity), Times.Once);
        }
    }
}
