namespace ProjectsLibrary.Domain.Exceptions
{
    public class CreatedEmployeeNotRegisteredException(string message, string details = "") : Exception(message)
    {
        public string Details { get; set; } = details;
    }
}

