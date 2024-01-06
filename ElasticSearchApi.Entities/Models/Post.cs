namespace ElasticSearchApi.Entities.Models
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Message { get; set; }
        public DateTime PostDate { get; set; } = DateTime.UtcNow;
        public string User { get; set; } = string.Empty;
    }
}
