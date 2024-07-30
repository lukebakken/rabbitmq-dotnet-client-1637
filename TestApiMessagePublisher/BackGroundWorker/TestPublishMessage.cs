using Newtonsoft.Json;
using RabbitMQHelper;

namespace TestApiMessagePublisher.BackGroundWorker
{
    public class TestPublishMessage : BackgroundService
    {
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(TestPublishMessage));
        //private readonly IServiceScopeFactory _serviceScopeFactory;
        string exchange = "testExchange";
        string testRoutingKey = "testRoutingKey";
        string TestTwoQueue= "testTwoQueue";
        string TestTwoRoutingKey= "testTwoRoutingKey";
        readonly IRabbitMQUtility rabbitMQUtility;
        public TestPublishMessage(IServiceScopeFactory factory, IRabbitMQUtility rabbitMQUtility,IConfiguration configuration)
        {
            //_serviceScopeFactory = factory;
            this.rabbitMQUtility = rabbitMQUtility;
            exchange = configuration["RabbitMQ:Exchange"];
            testRoutingKey = configuration["RabbitMQ:RoutingKey"];
        }

        protected async override Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            //_logger.ServiceStarted("Backgroundworker service started");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    for (int i = 0; i < 30000; i++)
                    {
                        rabbitMQUtility.Publish(JsonConvert.SerializeObject($"Test Message{i.ToString()}"), exchange, "topic", testRoutingKey);
                        rabbitMQUtility.Publish(JsonConvert.SerializeObject($"Test Two Message{i.ToString()}"), exchange, "topic", TestTwoRoutingKey);
                    }
                    await Task.Delay(new TimeSpan(0, 0, 5), cancellationToken);
                }
                catch (Exception ex)
                {
                    //_logger.ServiceFaulted(ex, list: "Publish error.");
                }
            }

            //_logger.ServiceFinishedSuccess("Backgroundworker completed.");
        }
    }
}
