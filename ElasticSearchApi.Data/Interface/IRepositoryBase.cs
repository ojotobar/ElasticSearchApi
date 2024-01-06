using ElasticSearchApi.Entities.DTO;

namespace ElasticSearchApi.Data.Interface
{
    public interface IRepositoryBase<TModel>
    {
        Task CreateAsync(TModel index);
        Task<ICollection<TModel>> GetAsync(SearchParameters search);
        Task<long> GetCountAsync(SearchParameters search);
        Task<TModel?> GetOneAsync(Guid id);
        Task<bool> RemoveAsync(Guid id);
        Task<bool> UpdateOneAsync(Guid id, TModel model);
    }
}
