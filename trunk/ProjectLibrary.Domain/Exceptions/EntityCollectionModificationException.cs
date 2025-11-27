namespace ProjectsLibrary.Domain.Exceptions
{
    public class EntityCollectionModificationException(string message, string details = "") : ProjectLibraryBaseException(message, details)
    {
    }
}
