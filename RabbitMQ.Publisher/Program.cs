using RabbitMQ.Client;
using System;
using System.Text;
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

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers; // Set the headers property
            properties.Persistent = true; // Make the message persistent

            channel.BasicPublish("header-exchange", string.Empty, properties,Encoding.UTF8.GetBytes("header message"));

            Console.WriteLine("Message sent to header-exchange with headers.");

            Console.ReadLine();
        }
    }
}