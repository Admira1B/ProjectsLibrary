using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.Domain.Contracts.Services {
    public interface IJwtService {
        string GenerateWebToken(Employee employee);
    }
}
