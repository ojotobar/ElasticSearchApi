using ElasticSearchApi.Entities.DTO;

namespace ElasticSearchApi.Data.Interface
{
    public interface IBaseRepositoryV1<TModel>
    {
        Task<long> CountAsync();
        Task<long> CountAsync(string propName, SearchParameters search);
        Task<bool> IndexAsync(TModel model);
        Task<bool> RemoveAsync(Guid id);
        Task<IEnumerable<TModel>> SearchAsync(string propName, SearchParameters search);
        Task<IEnumerable<TModel>> SearchAsync(SearchParameters search);
        Task<bool> UpdateAsync(Guid id, TModel model);
    }
}
