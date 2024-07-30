using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace TestWebAPI.RabbitMQ
{
    public class ConsumerBase:RabbitMqClientBase
    {
        private readonly IServiceScopeFactory factory;
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(ConsumerBase));

        public ConsumerBase(
            ConnectionFactory connectionFactory,
            IServiceScopeFactory factory, IConfiguration configuration) :
            base(connectionFactory, configuration)
        {
            this.factory = factory;
        }

        protected virtual async Task OnExternalEventReceived<T>(object sender, BasicDeliverEventArgs @event)
        {
            //_logger.ServiceStarted("External event received");
            try
            {
                using (var scope = factory.CreateScope())
                {
                    var body = Encoding.UTF8.GetString(@event.Body.ToArray());

                    var jsonString = JsonConvert.DeserializeObject<string>(body);
                    //Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                //_logger.ServiceFaulted(ex, "Error while retrieving message from queue.");
            }
            finally
            {
                //_logger.ServiceFinishedSuccess("Queue external source message acknowledgement sent", @event.DeliveryTag);

                Channel.BasicAck(@event.DeliveryTag, false);
            }
        }
        protected virtual async Task OnExternalEventTwoReceived<T>(object sender, BasicDeliverEventArgs @event)
        {
            //_logger.ServiceStarted("External event received");
            try
            {
                using (var scope = factory.CreateScope())
                {
                    var body = Encoding.UTF8.GetString(@event.Body.ToArray());

                    var jsonString = JsonConvert.DeserializeObject<string>(body);
                    //Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                //_logger.ServiceFaulted(ex, "Error while retrieving message from queue.");
            }
            finally
            {
                //_logger.ServiceFinishedSuccess("Queue external source message acknowledgement sent", @event.DeliveryTag);

                Channel.BasicAck(@event.DeliveryTag, false);
            }
        }
    }
}
