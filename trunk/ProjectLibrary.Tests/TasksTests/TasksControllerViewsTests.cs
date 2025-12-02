using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.MVC.Models.Task;

namespace ProjectLibrary.Tests.TasksTests {
    public class TasksControllerViewsTests : TasksControllerTests {
        [Fact]
        public async Task Index_WhenCalled_ReturnsView() {
            var user = MockClaimHelper.BuildEmployeeClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var model = new IndexTaskViewModel();

            _viewModelBuilder.Setup(b => b.BuildIndexViewModelAsync(user)).ReturnsAsync(model);

            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add_WhenCalled_ReturnsView() {
            var user = MockClaimHelper.BuildEmployeeClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var model = new AddTaskViewModel();

            _viewModelBuilder.Setup(b => b.BuildAddViewModelAsync(user, null, null)).ReturnsAsync(model);

            var result = await _controller.Add();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_WhenCalled_ReturnsView() {
            var user = MockClaimHelper.BuildEmployeeClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var requiredTaskid = 1;
            var model = new DetailsTaskViewModel();

            _viewModelBuilder.Setup(b => b.BuildDetailsViewModelAsync(requiredTaskid, user, null)).ReturnsAsync(model);

            var result = await _controller.Details(requiredTaskid);

            Assert.IsType<ViewResult>(result);
        }
    }
}
