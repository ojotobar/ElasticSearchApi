namespace ElasticSearchApi.Entities.DTO
{
    public class PostForCreate
    {
        public string? Message { get; set; }
        public string User { get; set; } = string.Empty;
    }
}
