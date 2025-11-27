using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.CompaniesTests
{
    public class CompaniesControllerAddTests : CompaniesControllerTests
    {
        [Fact]
        public async Task Add_WithValidDto_ReturnsRedirectToActionResult()
        {
            var companyAddDto = new CompanyAddDto
            {
                Name = "New Company",
            };

            var company = new Company
            {
                Name = "New Company",
            };

            _mapper.Setup(m => m.Map<Company>(companyAddDto))
                   .Returns(company);

            _companyService.Setup(s => s.AddAsync(company))
                          .Returns(Task.CompletedTask);

            var result = await _controller.Add(companyAddDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mapper.Verify(m => m.Map<Company>(companyAddDto), Times.Once);
            _companyService.Verify(s => s.AddAsync(company), Times.Once);
        }
    }
}
