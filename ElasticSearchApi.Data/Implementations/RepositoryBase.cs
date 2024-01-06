using ElasticSearchApi.Data.Interface;
using ElasticSearchApi.Entities.DTO;
using Nest;

namespace ElasticSearchApi.Data.Implementations
{
    public abstract class RepositoryBase<TModel> : IRepositoryBase<TModel> where TModel : class
    {
        private readonly IElasticClient client;

        public RepositoryBase(IElasticClient client)
        {
            this.client = client;
        }

        public async Task CreateAsync(TModel index) => 
            await client.IndexAsync(index, i => i.Index(typeof(TModel).Name.ToLower()));

        public async Task<ICollection<TModel>> GetAsync(SearchParameters search)
        {
            var result = await client.SearchAsync<TModel>(s => s
                .Index(typeof(TModel).Name.ToLower())
                .Query(q => q
                    .QueryString(s => s
                        .Query('*'+search.SearchBy+'*')))
                .From((search.PageNumber - 1) * search.PageSize)
                .Size(search.PageSize));

            var docs = result.Documents.ToList();
            return docs;
        }

        public async Task<TModel?> GetOneAsync(Guid id) =>
            (await client.GetAsync<TModel>(id, index => index.Index(typeof(TModel).Name.ToLower()))).Source;


        public async Task<long> GetCountAsync(SearchParameters search)
        {
            var result = await client.CountAsync<TModel>(s => s
                .Index(typeof(TModel).Name.ToLower())
                .Query(q => q
                    .QueryString(s => s
                        .Query('*'+search.SearchBy+'*'))));

            return result.Count;
        }

        public async Task<bool> UpdateOneAsync(Guid id, TModel model) =>
            (await client
                .UpdateAsync<TModel>(id, m => m
                    .Doc(model)
                    .Index(typeof(TModel).Name.ToLower())))
            .IsValid;

        public async Task<bool> RemoveAsync(Guid id) =>
            (await client
                .DeleteAsync<TModel>(id, m => m.Index(typeof(TModel).Name.ToLower()))).IsValid;
    }
}
