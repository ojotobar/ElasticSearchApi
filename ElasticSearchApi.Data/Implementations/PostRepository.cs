using ElasticSearchApi.Data.Interface;
using ElasticSearchApi.Entities.DTO;
using ElasticSearchApi.Entities.Models;
using Nest;

namespace ElasticSearchApi.Data.Implementations
{
    public sealed class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(IElasticClient client) : base(client) { }

        public async Task AddAsync(Post post) =>
            await CreateAsync(post);

        public async Task<bool> UpdateAsync(Guid id, Post post) =>
            await UpdateOneAsync(id, post);

        public async Task<bool> DeleteAsync(Guid id) =>
            await RemoveAsync(id);

        public async Task<Post?> FindAsync(Guid id) =>
            await GetOneAsync(id);

        public async Task<IEnumerable<Post>> FindAsync(SearchParameters search) =>
            await GetAsync(search);

        public async Task<long> CountAsync(SearchParameters search) =>
            await GetCountAsync(search);
    }
}
