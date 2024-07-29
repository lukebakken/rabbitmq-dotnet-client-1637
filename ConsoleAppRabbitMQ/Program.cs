// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;

Console.WriteLine("Testing started!");

var factory = new ConnectionFactory() 
{
    AutomaticRecoveryEnabled = true, // Enable automatic recovery
    NetworkRecoveryInterval = TimeSpan.FromSeconds(5), // Retry every 5 seconds
    //RequestedChannelMax = 10
};
factory.RequestedChannelMax = 3000;
using (var connection = factory.CreateConnection())
{
    int channelCount = 0;

    try
    {
        while (true)
        {
            var channel = connection.CreateModel();
            channelCount++;
            //if(channelCount>=10)
            //{
            //    channel.Close();
            //    channel.Dispose();
            //}
            //if (channelCount == 20)
            //{
            //    break;
            //}
            Console.WriteLine($"Channel {channelCount} created.");
        }
    }
    catch (RabbitMQ.Client.Exceptions.ChannelAllocationException ex)
    {
        Console.WriteLine($"Channel allocation exception caught: {channelCount}");
        Console.WriteLine(ex.Message);
    }
}
