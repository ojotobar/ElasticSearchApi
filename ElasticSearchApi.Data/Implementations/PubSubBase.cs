using ElasticSearchApi.Data.Interface;
using ElasticSearchApi.Entities.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ElasticSearchApi.Data.Implementations
{
    public abstract class PubSubBase<TMessage> : IPubSub<TMessage>
    {
        private readonly IConnection connection;
        private IModel channel;
        private ERabbitQueues _queue;

        public PubSubBase(IConnection connection, IModel channel)
        {
            this.connection = connection;
            this.channel = channel;
        }

        public void Publish(TMessage message, ERabbitExchanges exchange, ERabbitQueues queue)
        {
            ExchangeInit(exchange);
            QueueInit(queue);
            QueueBind(queue, exchange);

            using var channel = connection.CreateModel();
            var jsonString = JsonSerializer.Serialize(message);
            var encodedMessage = Encoding.UTF8.GetBytes(jsonString);
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            channel.BasicPublish(exchange.ToString(), queue.ToString().ToLower(), properties, encodedMessage);
        }

        public void Subscribe(ERabbitQueues queue)
        {
            _queue = queue;
            channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);

            var channelConsumer = new AsyncEventingBasicConsumer(channel);
            channelConsumer.Received += ReceivedChannelConsumer;

            channel.BasicConsume(queue.ToString(), false, channelConsumer);
        }

        private async Task ReceivedChannelConsumer(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            await Task.Run(() => LogMessage(_queue, message));
            channel.BasicAck(e.DeliveryTag, false);
        }

        private void ExchangeInit(ERabbitExchanges exchange)
        {
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange.ToString(), ExchangeType.Direct, true, false);
        }

        private void QueueInit(ERabbitQueues queue)
        {
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue.ToString(), true, false, false);
        }

        private void QueueBind(ERabbitQueues queue, ERabbitExchanges exchange)
        {
            using var channel = connection.CreateModel();
            channel.QueueBind(queue.ToString(), exchange.ToString(), queue.ToString().ToLower());
        }

        private void LogMessage(ERabbitQueues queue, string message)
        {
            var dObj = JsonSerializer.Deserialize<TMessage>(message);
            switch (queue)
            {
                case ERabbitQueues.PostLog:
                    Console.WriteLine($"The post {dObj} has been logged successfully");
                    break;
                default:
                    break;
            }
        }
    }
}
