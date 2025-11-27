namespace ProjectsLibrary.Domain.Exceptions {
    public class JwtOptionsNotConfiguratedException(string message, string details = "") : ProjectLibraryBaseException(message, details) {
    }
}
