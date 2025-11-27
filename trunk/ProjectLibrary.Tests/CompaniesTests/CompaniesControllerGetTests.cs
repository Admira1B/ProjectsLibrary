using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Domain.Models.RequestModels;

namespace ProjectLibrary.Tests.CompaniesTests {
    public class CompaniesControllerGetTests : CompaniesControllerTests {
        [Fact]
        public async Task Get_WithValidModel_ReturnsJsonResult() {
            var model = new GetPagedModel {
                Draw = 1,
                Start = 0,
                Length = 10,
                SearchingValue = "test",
                SearchableFieldsNames = ["id", "name"],
                SortColumn = "id",
                SortDirection = "asc"
            };

            var companies = new List<Company>
            {
                new() { Id = 1, Name = "Test Company 1" },
                new() { Id = 2, Name = "Test Company 2" }
            };

            var pagedResult = new PagedResult<Company> {
                Datas = companies,
                TotalRecords = 100,
                FilteredRecords = 50
            };

            var companyDtos = new List<CompanyReadDto>
            {
                new() { Id = 1, Name = "Test Company 1" },
                new() { Id = 2, Name = "Test Company 2" }
            };

            _companyService.Setup(s => s.GetPaginatedAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<SortParams>(),
                It.IsAny<PageParams>()))
                .ReturnsAsync(pagedResult);

            _mapper.Setup(m => m.Map<List<CompanyReadDto>>(companies))
                   .Returns(companyDtos);

            var result = await _controller.Get(model);

            Assert.IsType<JsonResult>(result);
        }
    }
}
