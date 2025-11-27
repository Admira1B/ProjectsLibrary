namespace ProjectsLibrary.Domain.Exceptions {
    public class EmailNotRegisteredException(string message, string details = "") : ProjectLibraryBaseException(message, details) {
    }
}

