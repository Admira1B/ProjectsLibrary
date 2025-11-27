namespace ProjectsLibrary.Domain.Exceptions
{
    public class EntityNotFoundException(string message, string details = "") : ProjectLibraryBaseException(message, details)
    {
    }
}
