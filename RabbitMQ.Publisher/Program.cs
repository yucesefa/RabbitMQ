using RabbitMQ.Client;
using System;
namespace RabbitMQ.Publisher
{
    public enum LogNames
    {
        Critical=1,
        Error = 2,
        Warning = 3,
        Info = 4
    }
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.Uri =  new Uri("amqps://imoearbm:SKLx7G1hs1_fNyGVw3MMDlc6bNLihq8x@woodpecker.rmq.cloudamqp.com/imoearbm");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // channel.QueueDeclare("hello-queue", true, false, false); routing key is not used in this case (fanout)

            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            //Enum.GetNames(typeof(LogNames)).ToList().ForEach(logName =>
            //{
            //    Random random = new Random();
            //    LogNames log1 = (LogNames)random.Next(1, 5);
            //    LogNames log2 = (LogNames)random.Next(1, 5);
            //    LogNames log3 = (LogNames)random.Next(1, 5);


            //    var routeKey = $"{log1}.{log2}.{log3}";
            //    //var queueName = $"direct-queue-{logName}";
            //    //channel.QueueDeclare(queueName, true, false, false);

            //    channel.QueueBind(queueName, "logs-direct", routeKey, null);
            //});
            Random random = new Random();
            Enumerable.Range(1,50).ToList().ForEach(i =>
            {
                LogNames log =(LogNames)new Random().Next(1, 4); 

    
                LogNames log1 = (LogNames)random.Next(1, 5);
                LogNames log2 = (LogNames)random.Next(1, 5);
                LogNames log3 = (LogNames)random.Next(1, 5);

                var routeKey = $"{log1}.{log2}.{log3}";

                string message = $"log-type: {log1}-{log2}-{log3} - Things Of Quality Have No Fear Of Time (topic)";

                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-topic",routeKey,null, messageBody);
                Console.WriteLine($"Log sent  {message}");
            });
            Console.ReadLine();
        }
    }
}