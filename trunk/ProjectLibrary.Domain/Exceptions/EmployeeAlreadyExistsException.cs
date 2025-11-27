namespace ProjectsLibrary.Domain.Exceptions {
    public class EmployeeAlreadyExistsException(string message, string details = "") : ProjectLibraryBaseException(message, details) {
    }
}
