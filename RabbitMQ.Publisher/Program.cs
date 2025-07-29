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

            channel.QueueDeclare("hello-queue", true, false, false);

            string message = "Things Of Quality Have No Fear Of Time";

            var messageBody = System.Text.Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(string.Empty,"hello-queue",null,messageBody);
            Console.WriteLine("Message sent");
            Console.ReadLine();
        }
    }
}