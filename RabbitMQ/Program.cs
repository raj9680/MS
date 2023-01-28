using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

/* https://easynetq.com 
 * https://masstransit-project.com for easy wrapper on RabbitMq
 */

namespace RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteMessage();
        }

        #region For RabbitMQ

        /* For Writing
         */
        static void WriteMessage()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();
            var properties = model.CreateBasicProperties();
            properties.Persistent = false;

            byte[] messageBuffer = Encoding.Default.GetBytes("Direct Message");
            model.BasicPublish("My Exchange Name", "1001", properties, messageBuffer);
            Console.WriteLine("Message Sent");
        }

        /* For Reading
         */
        static void ReadMessage()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [X] Received {0}", message);
                };

                channel.BasicConsume(
                    queue: "InventoryQueue",
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit");
            }
        }
        #endregion
    }
}
