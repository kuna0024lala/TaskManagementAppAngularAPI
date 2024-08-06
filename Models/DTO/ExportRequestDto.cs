namespace TaskManagementAppAngular.Models.DTO
{
    public class ExportRequestDto
    {
        public string SearchQuery { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<int> SelectedIds { get; set; }
    }
}
