using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using TestWebAPI.Models;

namespace TestWebAPI.RabbitMQ
{
    public class LogConsumer:ConsumerBase,IHostedService
    {
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(LogConsumer));
        private readonly IConfiguration configuration;
        public LogConsumer(
            ConnectionFactory connectionFactory,
            IServiceScopeFactory factory, IConfiguration configuration) :
            base(connectionFactory, factory, configuration)
        {
            this.configuration = configuration;
            CreateConsumers();
        }
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;
        }


        private void CreateConsumers()
        {
            //_logger.ServiceProcessing("Create consumers started");

            try
            {
                Channel.BasicQos(0, 150, false);

                CreateExternalSourceConsumer();
                CreateExternalSourceTwoConsumer();
            }
            catch (Exception ex)
            {
                //_logger.ServiceFaulted(ex, "Error while creating consumers");
            }
        }

        private void CreateExternalSourceConsumer()
        {
            try
            {
                //_logger.ServiceProcessing("Create external source consumers started");

                var consumer = new AsyncEventingBasicConsumer(Channel);
                consumer.Received += OnExternalEventReceived<TestMessage>;
                Channel.BasicConsume(queue: TestQueue, autoAck: false, consumer: consumer);
                consumer.ConsumerCancelled += OnExternalSourceConsumerCancel;
            }
            catch (Exception ex)
            {
                //_logger.ServiceFaulted(ex, "Error in creating external source consumers");

                throw;
            }
        }

        private async Task OnExternalSourceConsumerCancel(object sender, ConsumerEventArgs @even)
        {
            //_logger.ServiceProcessing("External source consumer cancel event triggered");

            CreateExternalSourceConsumer();
        }
        private void CreateExternalSourceTwoConsumer()
        {
            try
            {
                //_logger.ServiceProcessing("Create external source consumers started");

                var consumer = new AsyncEventingBasicConsumer(Channel);
                consumer.Received += OnExternalEventTwoReceived<TestMessage>;
                Channel.BasicConsume(queue: TestTwoQueue, autoAck: false, consumer: consumer);
                consumer.ConsumerCancelled += OnExternalSourceTwoConsumerCancel;
            }
            catch (Exception ex)
            {
                //_logger.ServiceFaulted(ex, "Error in creating external source consumers");

                throw;
            }
        }

        private async Task OnExternalSourceTwoConsumerCancel(object sender, ConsumerEventArgs @even)
        {
            //_logger.ServiceProcessing("External source consumer cancel event triggered");

            CreateExternalSourceTwoConsumer();
        }
    }
}
