using ElasticSearchApi.Data.Interface;
using ElasticSearchApi.Entities.Enums;
using ElasticSearchApi.Entities.Models;
using RabbitMQ.Client;

namespace ElasticSearchApi.Data.Implementations
{
    public class PostPubSub : PubSubBase<Post>, IPostPubSub
    {
        public PostPubSub(IConnection connection, IModel channel) 
            : base(connection, channel){}

        public void PublishPost(Post message, ERabbitExchanges exchange, ERabbitQueues queue) =>
            Publish(message, exchange, queue);

        public void SubscribeToPost(ERabbitQueues queue) =>
            Subscribe(queue);
    }
}
