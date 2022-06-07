using System.Collections.Generic;
using System.ComponentModel;

namespace Sophie.Resource.Model
{
    public class Paging
    {
        [DefaultValue(99)]
        public int? PageSize { get; set; } = 99;

        [DefaultValue(0)]
        public int? PageIndex { get; set; } = 0;

        [DefaultValue("")]
        public string? search { get; set; } = "";

        [DefaultValue("Updated")]
        public string? sortName { get; set; } = "Updated";

        [DefaultValue("desc")]
        public string? sort { get; set; } = "desc";
    }

    public class PagingResult<T>
    {
        public int Total { get; set; }
        public List<T> Result { get; set; }
    }

    public class FilterWithId : Paging
    {
        [DefaultValue("")]
        public string Id { get; set; }
    }
}
