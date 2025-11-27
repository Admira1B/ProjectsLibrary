namespace ProjectsLibrary.Domain.Exceptions {
    public class IncorrectEmployeePasswordException(string message, string details = "") : ProjectLibraryBaseException(message, details) {
    }
}
