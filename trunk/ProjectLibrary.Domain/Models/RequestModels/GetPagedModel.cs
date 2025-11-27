
namespace ProjectsLibrary.Domain.Models.RequestModels {
    public class GetPagedModel {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string SortColumn { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;
        public string SearchingValue { get; set; } = string.Empty;
        public string[] SearchableFieldsNames { get; set; } = [];
    }
}
