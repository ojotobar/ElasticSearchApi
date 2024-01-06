using ElasticSearchApi.Entities.Enums;
using ElasticSearchApi.Entities.Models;

namespace ElasticSearchApi.Data.Interface
{
    public interface IPostPubSub
    {
        void PublishPost(Post message, ERabbitExchanges exchange, ERabbitQueues queue);
        void SubscribeToPost(ERabbitQueues queue);
    }
}
