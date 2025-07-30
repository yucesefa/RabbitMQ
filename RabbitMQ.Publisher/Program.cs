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

            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Fanout);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(logName =>
            {
                var routeKey = $"route-{logName}";
                var queueName = $"direct-queue-{logName}";
                channel.QueueDeclare(queueName, true, false, false);

                channel.QueueBind(queueName, "logs-direct", routeKey, null);
            });

            Enumerable.Range(1,50).ToList().ForEach(i =>
            {
                LogNames log =(LogNames)new Random().Next(1, 4); 

                string message = $"log-type {log} - Things Of Quality Have No Fear Of Time (direct)";

                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{log}";
                
                channel.BasicPublish("logs-direct",routeKey,null, messageBody);
                Console.WriteLine($"Log sent  {message}");
            });
            Console.ReadLine();
        }
    }
}