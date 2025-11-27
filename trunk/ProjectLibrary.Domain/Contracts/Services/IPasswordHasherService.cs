namespace ProjectsLibrary.Domain.Contracts.Services {
    public interface IPasswordHasherService {
        string GetPasswordHash(string password);

        bool VerifyPassword(string password, string passwordHash);
    }
}
