// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var msg = "Hello World!";

var queueName = "myqueue";
var exchangeName = "myexchange";

var factory = new ConnectionFactory()
{
    HostName = "192.168.50.10",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = new TimeSpan(0, 1, 0),
    RequestedHeartbeat = new TimeSpan(0, 0, 600)
};
using var con = factory.CreateConnection();
using var channel = con.CreateModel();

// send
channel.QueueDeclare(queueName, true, false, false, null);
channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true, false, null);
channel.QueueBind(queueName, exchangeName, "");
channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(msg));
Console.WriteLine($"Send: {msg}");

// recieve
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);
};
channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);
Console.ReadKey();