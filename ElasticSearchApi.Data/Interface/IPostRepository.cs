using ElasticSearchApi.Entities.DTO;
using ElasticSearchApi.Entities.Models;

namespace ElasticSearchApi.Data.Interface
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task<long> CountAsync(SearchParameters search);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Post>> FindAsync(SearchParameters search);
        Task<Post?> FindAsync(Guid id);
        Task<bool> UpdateAsync(Guid id, Post post);
    }
}
