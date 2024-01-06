using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using ElasticSearchApi.Data.Configurations;
using ElasticSearchApi.Data.Interface;
using ElasticSearchApi.Entities.DTO;
using Microsoft.Extensions.Options;

namespace ElasticSearchApi.Data.Implementations
{
    public abstract class RepositoryBaseV1<TModel> : IBaseRepositoryV1<TModel> where TModel : class
    {
        private readonly ElasticsearchClient client;

        public RepositoryBaseV1(IOptions<ElasticSettings> options)
        {
            var settings = new ElasticsearchClientSettings(new Uri(options.Value.Cloud))
                .Authentication(new BasicAuthentication(options.Value.Key, options.Value.Value));
            client = new ElasticsearchClient(settings);
        }

        public async Task<bool> IndexAsync(TModel model) =>
            (await client.IndexAsync(model, (nameof(TModel)).ToLower())).IsValidResponse;
            

        public async Task<TModel?> GetAsync(Guid id)
        {
            var response = await client.GetAsync<TModel>(id, index => index.Index((nameof(TModel)).ToLower()));

            return response.IsValidResponse ? response.Source : null;
        }

        public async Task<IEnumerable<TModel>> SearchAsync(string propName, SearchParameters search)
        {
            var response = await client.SearchAsync<TModel>(s => s
                    .Index(nameof(TModel).ToLower())
                    .From((search.PageNumber - 1) * search.PageNumber)
                    .Size(search.PageSize)
                    .Query(q => q
                        .MatchPhrase(m => m
                            .Field(propName).Query(search.SearchBy))));


            return response.IsValidResponse ? 
                response.Documents.ToList() : 
                new List<TModel>();
        }

        public async Task<IEnumerable<TModel>> SearchAsync(SearchParameters search)
        {
            var response = await client.SearchAsync<TModel>(s => s
                    .Index(nameof(TModel).ToLower())
                    .From((search.PageNumber - 1) * search.PageNumber)
                    .Size(search.PageSize));


            return response.IsValidResponse ?
                response.Documents.ToList() :
                new List<TModel>();
        }

        public async Task<bool> UpdateAsync(Guid id, TModel model) => 
            (await client
                .UpdateAsync<TModel, TModel>(nameof(TModel).ToLower(), id, u => u.Doc(model)))
                    .IsValidResponse;

        public async Task<bool> RemoveAsync(Guid id) =>
            (await client
                .DeleteAsync(nameof(TModel).ToLower(), id))
                    .IsValidResponse;

        public async Task<long> CountAsync(string propName, SearchParameters search)
        {
            var response = await client.CountAsync<TModel>(c => c
                .Indices(nameof(TModel).ToLower())
                .Query(q => q
                        .MatchPhrase(m => m
                            .Field(propName).Query(search.SearchBy))));

            return response.IsValidResponse ? 
                response.Count : 
                default;
        }

        public async Task<long> CountAsync()
        {
            var response = await client.CountAsync<TModel>(c => c
                .Indices(nameof(TModel).ToLower()));

            return response.IsValidResponse ?
                response.Count :
                default;
        }
    }
}
