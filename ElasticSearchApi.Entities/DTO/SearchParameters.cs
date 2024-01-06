namespace ElasticSearchApi.Entities.DTO
{
    public class SearchParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchBy { get; set; } = string.Empty;
    }
}
