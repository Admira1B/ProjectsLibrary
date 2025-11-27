using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjectsLibrary.MVC.Models.Employee;

namespace ProjectLibrary.Tests.EmployeesTests {
    public class EmployeeControllerViewsTests : EmployeeControllerTests {
        [Fact]
        public async Task Index_WithRequiredRole_ReturnsView() {
            var user = MockClaimHelper.BuildSupervisorClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var model = new IndexEmployeeViewModel();

            _viewModelBuilder.Setup(b => b.BuildIndexViewModelAsync(user)).ReturnsAsync(model);

            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add_WithRequiredRole_ReturnsView() {
            var user = MockClaimHelper.BuildSupervisorClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var model = new AddEmployeeViewModel();

            _viewModelBuilder.Setup(b => b.BuildAddViewModelAsync(null)).ReturnsAsync(model);

            var result = await _controller.Add();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_WithRequiredRole_ReturnsView() {
            var user = MockClaimHelper.BuildSupervisorClaim();

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var requredEmployeeId = 1;

            var model = new DetailsEmployeeViewModel();

            _viewModelBuilder.Setup(b => b.BuildDetailsViewModelAsync(requredEmployeeId, null)).ReturnsAsync(model);

            var result = await _controller.Details(requredEmployeeId);

            Assert.IsType<ViewResult>(result);
        }
    }
}
