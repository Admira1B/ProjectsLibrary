namespace ProjectsLibrary.Domain.Models.RequestModels {
    public class FilterParams {
        public string SearchingValue { get; set; } = string.Empty;
        public string[] SearchableFieldsNames { get; set; } = [];
    }
}
