using ElasticSearchApi.Entities.Enums;

namespace ElasticSearchApi.Data.Interface
{
    public interface IPubSub<TMessage>
    {
        void Publish(TMessage message, ERabbitExchanges exchange, ERabbitQueues queue);
        void Subscribe(ERabbitQueues queue);
    }
}
