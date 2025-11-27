using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.CompaniesTests
{
    public class CompaniesControllerUpdateTests : CompaniesControllerTests
    {
        [Fact]
        public async Task Update_WithValidData_ReturnsRedirectToAction()
        {
            var id = 1;
            var companyUpdateDto = new CompanyUpdateDto
            {
                Name = "Updated Company",
            };
            var companyEntity = new Company
            {
                Name = "Updated Company",
            };

            _mapper.Setup(m => m.Map<Company>(companyUpdateDto))
                   .Returns(companyEntity);
            _companyService.Setup(s => s.UpdateAsync(It.Is<Company>(c => c.Id == id)))
                          .Returns(Task.CompletedTask);

            var result = await _controller.Update(id, companyUpdateDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mapper.Verify(m => m.Map<Company>(companyUpdateDto), Times.Once);
            _companyService.Verify(s => s.UpdateAsync(It.Is<Company>(c => c.Id == id)), Times.Once);
        }
    }
}
