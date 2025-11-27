using Moq;
using Microsoft.AspNetCore.Mvc;

namespace ProjectLibrary.Tests.CompaniesTests
{
    public class CompaniesControllerDeleteTests : CompaniesControllerTests
    {
        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            var id = 1;
            _companyService.Setup(s => s.DeleteAsync(id))
                          .Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _companyService.Verify(s => s.DeleteAsync(id), Times.Once);
        }
    }
}
