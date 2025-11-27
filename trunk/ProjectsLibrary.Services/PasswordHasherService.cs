using ProjectsLibrary.Domain.Contracts.Services;

namespace ProjectsLibrary.Services {
    public class PasswordHasherService : IPasswordHasherService {
        public string GetPasswordHash(string password) {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash) {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }
    }
}
