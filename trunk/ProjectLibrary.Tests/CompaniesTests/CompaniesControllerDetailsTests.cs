using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.MVC.Models.Company;

namespace ProjectLibrary.Tests.CompaniesTests {
    public class CompaniesControllerDetailsTests : CompaniesControllerTests {
        [Fact]
        public async Task Details_WithValidData_RedirectToIndex() {
            var model = new DetailsCompanyViewModel() {
                Id = 1,
                Company = new CompanyUpdateDto() { Name = "CompanyName" },
            };

            var company = new Company();

            _mapper.Setup(m => m.Map<Company>(model.Company)).Returns(company);

            var result = await _controller.Details(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _mapper.Verify(m => m.Map<Company>(model.Company), Times.Once);
        }
    }
}
