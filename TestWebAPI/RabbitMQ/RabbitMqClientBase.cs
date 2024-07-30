using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TestWebAPI.RabbitMQ
{
    public abstract class RabbitMqClientBase:IDisposable
    {
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(RabbitMqClientBase));
        protected string Exchange;
        protected readonly string TestQueue;
        protected readonly string TestRoutingKey;
        protected readonly string TestTwoQueue;
        protected readonly string TestTwoRoutingKey;
            
        protected IModel Channel { get; private set; }
        private IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;

        protected RabbitMqClientBase(
            ConnectionFactory connectionFactory, IConfiguration configuration)
        {
            Exchange = "testExchange";
            TestQueue = "testQueue";
            TestRoutingKey = "testRoutingKey";
            TestTwoQueue = "testTwoQueue";
            TestTwoRoutingKey = "testTwoRoutingKey";
            _connectionFactory = connectionFactory;
            ConnectToRabbitMq();
        }

        private void ConnectToRabbitMq()
        {
            try
            {
                //_logger.ServiceStarted("Start attempting connection to rabbit mq");

                if (_connection == null || _connection.IsOpen == false)
                {
                    //_logger.ServiceProcessing("Create connection to rabbit mq");

                    _connectionFactory.AutomaticRecoveryEnabled = true;
                    _connectionFactory.TopologyRecoveryEnabled= false;
                    _connection = _connectionFactory.CreateConnection();
                    //_connection.ConnectionShutdown += Connection_ConnectionShutdown;
                }

                //_logger.ServiceProcessing("Create channel model and bind queue in rabbit mq");
                if (Channel == null || Channel.IsOpen == false)
                {
                    Channel = _connection.CreateModel();
                    Channel.ExchangeDeclare(
                        exchange: Exchange,
                        type: "topic",
                        durable: true,
                        autoDelete: false);

                    Channel.QueueDeclare(
                        queue: TestQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false);

                    Channel.QueueBind(
                        queue: TestQueue,
                        exchange: Exchange,
                        routingKey: TestRoutingKey);

                    Channel.ExchangeDeclare(
                        exchange: Exchange,
                        type: "topic",
                        durable: true,
                        autoDelete: false);

                    Channel.QueueDeclare(
                        queue: TestTwoQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false);

                    Channel.QueueBind(
                        queue: TestTwoQueue,
                        exchange: Exchange,
                        routingKey: TestTwoRoutingKey);
                }
            }
            catch (Exception ex)
            {
                //_logger.ServiceFaulted(ex);
            }
        }

        public void Dispose()
        {
            //_logger.ServiceProcessing("Rabbit mq connection and channel dispose");

            try
            {
                Channel?.Close();
                Channel?.Dispose();
                Channel = null;

                _connection?.Close();
                _connection?.Dispose();
                _connection = null;
            }
            catch (Exception ex)
            {
                //_logger.ServiceFaulted(ex, "Rabbit mq connection and channel dispose failed");
            }
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            //_logger.ServiceProcessing("Rabbit mq connection shutdown event handler called");
            Cleanup();

            while (true)
            {
                try
                {
                    ConnectToRabbitMq();

                    //_logger.ServiceFinishedSuccess("Rabbit mq connection reconnected successfully");

                    break;
                }
                catch (Exception ex)
                {
                    //_logger.ServiceFaulted(ex, "Rabbit mq reconnection failed");

                    Thread.Sleep(3000);
                }
            }
        }

        private void Cleanup()
        {
            //_logger.ServiceProcessing("Close existing rabbit mq channel if exists");

            try
            {
                if (Channel != null && Channel.IsOpen)
                {
                    Channel.Close();
                    Channel = null;
                }

                if (_connection != null && _connection.IsOpen)
                {
                    _connection.Close();
                    _connection = null;
                }
            }
            catch (IOException ex)
            {
                //_logger.ServiceFaulted(ex, "Rabbit mq connection close failed");
            }
        }
    }
}
