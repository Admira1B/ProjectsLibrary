using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjectsLibrary.MVC.Models.Company;

namespace ProjectLibrary.Tests.CompaniesTests
{
    public class CompaniesControllerViewsTests : CompaniesControllerTests
    {
        [Fact]
        public async Task Index_WhenCalled_ReturnsView()
        {
            var user = MockClaimHelper.BuildSupervisorClaim();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var model = new IndexCompanyViewModel();

            _viewModelBuilder.Setup(b => b.BuildIndexViewModelAsync(null)).ReturnsAsync(model);

            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_WhenCalled_ReturnsView()
        {
            var user = MockClaimHelper.BuildSupervisorClaim();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var requiredCompanyId = 1;

            var model = new DetailsCompanyViewModel();

            _viewModelBuilder.Setup(b => b.BuildDetailsViewModelAsync(requiredCompanyId, null)).ReturnsAsync(model);

            var result = await _controller.Details(requiredCompanyId);

            Assert.IsType<ViewResult>(result);
        }
    }
}
