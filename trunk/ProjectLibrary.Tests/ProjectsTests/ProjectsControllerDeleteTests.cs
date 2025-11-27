using Moq;
using Microsoft.AspNetCore.Mvc;

namespace ProjectLibrary.Tests.ProjectsTests
{
    public class ProjectsControllerDeleteTests : ProjectsControllerTests
    {
        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            var id = 1;
            _projectService.Setup(x => x.DeleteAsync(id))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
            _projectService.Verify(x => x.DeleteAsync(id), Times.Once);
        }
    }
}
