namespace ProjectsLibrary.Domain.Exceptions {
    public class PropertyNotFoundException(string message, string details = "") : ProjectLibraryBaseException(message, details) {
    }
}
