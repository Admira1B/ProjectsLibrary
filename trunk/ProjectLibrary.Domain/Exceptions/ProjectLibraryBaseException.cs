namespace ProjectsLibrary.Domain.Exceptions {
    public abstract class ProjectLibraryBaseException(string message, string details = "") : Exception(message) {
        public string Details { get; set; } = details;
    }
}
