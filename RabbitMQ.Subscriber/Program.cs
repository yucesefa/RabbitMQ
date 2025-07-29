using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
namespace RabbitMQ.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://imoearbm:SKLx7G1hs1_fNyGVw3MMDlc6bNLihq8x@woodpecker.rmq.cloudamqp.com/imoearbm");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare("hello-queue", true, false, false);

            channel.BasicQos(0, 1, false); // Fair dispatch - only one message at a time per consumer

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue", false, consumer);

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Received Message = " + message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}