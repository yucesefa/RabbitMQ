using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Shared;
using System;
using System.Text.Json;
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
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            //channel.QueueDeclare("hello-queue", true, false, false);

            var randomQueueName = channel.QueueDeclare().QueueName; // Create a temporary queue

            channel.QueueBind(randomQueueName, "logs-fanout", "",null); // Bind the temporary queue to the fanout exchange

            
            
            channel.BasicQos(0, 1, false); // Fair dispatch - only one message at a time per consumer

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName; // Create a new queue for the consumer

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "any"); // Use "all" for matching all headers

            channel.QueueBind(queueName, "header-exchange",String.Empty,headers);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Waiting for logs... Press Enter to exit.");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());

                Product product = JsonSerializer.Deserialize<Product>(message);

                Console.WriteLine($"Received Message =  { product.Id}- {product.Name}- {product.Price}-{product.Stock}");

               // File.AppendAllText("logs.txt", $"{DateTime.Now}: {message}\n");

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}