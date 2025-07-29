using RabbitMQ.Client;
using System;
namespace RabbitMQ.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.Uri =  new Uri("amqps://imoearbm:SKLx7G1hs1_fNyGVw3MMDlc6bNLihq8x@woodpecker.rmq.cloudamqp.com/imoearbm");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // channel.QueueDeclare("hello-queue", true, false, false); routing key is not used in this case (fanout)

            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1,50).ToList().ForEach(i =>
            {
                string message = $"Message {i} - Things Of Quality Have No Fear Of Time (fanout)";
                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-fanout","",null, messageBody);
                Console.WriteLine($"Message {i} sent");
            });
            Console.ReadLine();
        }
    }
}