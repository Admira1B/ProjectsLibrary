namespace ProjectsLibrary.Domain.Models.Results {
    public class PagedResult<T> {
        public List<T> Datas { get; set; } = [];
        public int FilteredRecords { get; set; }
        public int TotalRecords { get; set; }
    }
}
