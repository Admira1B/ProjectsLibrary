using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Company;

namespace ProjectLibrary.Tests.CompaniesTests {
    public class CompaniesControllerApiTests : CompaniesControllerTests {
        [Fact]
        public async Task Get_WithValidModel_ReturnsOk() {
            var companies = new List<Company>();
            var companyDtos = new List<CompanyReadDto>();

            var model = new GetPagedModel {
                Draw = 1,
                Start = 0,
                Length = 10,
                SearchingValue = "test",
                SearchableFieldsNames = ["id", "name"],
                SortColumn = "id",
                SortDirection = "asc"
            };

            var pagedResult = new PagedResult<Company> {
                Datas = companies,
                TotalRecords = 100,
                FilteredRecords = 50
            };


            _companyService.Setup(s => s.GetPaginatedAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<SortParams>(),
                It.IsAny<PageParams>()))
                .ReturnsAsync(pagedResult);

            _mapper.Setup(m => m.Map<List<CompanyReadDto>>(companies))
                   .Returns(companyDtos);

            var result = await _apiController.Get(model);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Add_WithValidDto_ReturnsOk() {
            var company = new Company();
            var companyDto = new CompanyAddDto() { Name = "CompanyName" };

            _mapper.Setup(m => m.Map<Company>(companyDto)).Returns(company);

            var result = await _apiController.Add(companyDto);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent() {
            var id = 1;
            var result = await _apiController.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
