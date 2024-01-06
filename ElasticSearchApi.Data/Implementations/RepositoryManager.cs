using ElasticSearchApi.Data.Interface;
using Nest;
using RabbitMQ.Client;

namespace ElasticSearchApi.Data.Implementations
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IPostRepository> postRepository;
        private readonly Lazy<IPostPubSub> postPubSub;

        public RepositoryManager(IElasticClient client, IConnection connection, IModel model)
        {
            postRepository = new Lazy<IPostRepository>(() =>
                new PostRepository(client));
            postPubSub = new Lazy<IPostPubSub>(() =>
                new PostPubSub(connection, model));
        }

        public IPostRepository Post => postRepository.Value;

        public IPostPubSub PostPubSub => postPubSub.Value;
    }
}
