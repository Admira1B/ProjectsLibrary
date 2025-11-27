using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.MVC.Models.Project;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerViewsTests : ProjectsControllerTests {
        [Fact]
        public async Task Index_WhenCalled_ReturnsView() {
            var user = MockClaimHelper.BuildManagerClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var model = new IndexProjectViewModel();

            _viewModelBuilder.Setup(b => b.BuildIndexViewModelAsync(user)).ReturnsAsync(model);

            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add_WhenCalled_ReturnsView() {
            var user = MockClaimHelper.BuildManagerClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var model = new AddProjectViewModel();

            _viewModelBuilder.Setup(b => b.BuildAddViewModelAsync(null)).ReturnsAsync(model);

            var result = await _controller.Add();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_WhenCalled_ReturnsView() {
            var user = MockClaimHelper.BuildManagerClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var requredProjectId = 1;

            var model = new DetailsProjectViewModel();

            _viewModelBuilder.Setup(b => b.BuildDetailsViewModelAsync(requredProjectId, null)).ReturnsAsync(model);

            var result = await _controller.Details(requredProjectId);

            Assert.IsType<ViewResult>(result);
        }
    }
}
