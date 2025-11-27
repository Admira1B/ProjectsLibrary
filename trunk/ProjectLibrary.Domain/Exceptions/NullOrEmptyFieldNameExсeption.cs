namespace ProjectsLibrary.Domain.Exceptions {
    public class NullOrEmptyFieldNameExсeption(string message, string details = "") : ProjectLibraryBaseException(message, details) {
    }
}
