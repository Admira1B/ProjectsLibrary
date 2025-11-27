namespace ProjectsLibrary.Domain.Exceptions
{
    public class UnknownUserRoleException(string message, string details = "") : ProjectLibraryBaseException(message, details)
    {
    }
}

