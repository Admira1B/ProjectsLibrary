using ProjectsLibrary.Domain.Models.Enums;

namespace ProjectsLibrary.Domain.Models.RequestModels
{
    public class SortParams
    {
        public string OrderByParam { get; set; } = string.Empty;
        public SortDirection Direction { get; set; }
    }
}
