namespace ProjectsLibrary.Domain.Exceptions {
    public class NoRoleClaimToCurrentUserException(string message, string details = "") : ProjectLibraryBaseException(message, details) {
    }
}
