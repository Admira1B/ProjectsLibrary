namespace ProjectsLibrary.Domain.Models.Enums
{
    public enum TaskPLStatus
    {
        ToDo = 0,
        InProgress,
        Done
    }

    public static class TaskPLStatusExtension 
    {
        public static string StatusToString(this TaskPLStatus status) 
        {
            return status switch
            {
                TaskPLStatus.ToDo => "To do",
                TaskPLStatus.InProgress => "In progress",
                TaskPLStatus.Done => "Done",
                _ => status.ToString(),
            };
        }
    }
}
