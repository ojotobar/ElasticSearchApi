namespace ElasticSearchApi.Data.Interface
{
    public interface IRepositoryManager
    {
        IPostRepository Post { get; }
        IPostPubSub PostPubSub { get; }
    }
}
