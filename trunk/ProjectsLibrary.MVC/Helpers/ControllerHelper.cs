using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;

namespace ProjectsLibrary.MVC.Helpers {
    public static class ControllerHelper {
        public static (FilterParams filterParams, SortParams sortParams, PageParams pageParams) BuildGetMethodModelParams(GetPagedModel model) {
            var filterParams = new FilterParams() {
                SearchingValue = model.SearchingValue,
                SearchableFieldsNames = model.SearchableFieldsNames
            };

            var sortParams = new SortParams() {
                Direction = model.SortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending,
                OrderByParam = model.SortColumn
            };

            var pageParams = new PageParams() {
                PageSize = model.Length,
                Skip = model.Start,
            };

            return (filterParams, sortParams, pageParams);
        }
    }
}
