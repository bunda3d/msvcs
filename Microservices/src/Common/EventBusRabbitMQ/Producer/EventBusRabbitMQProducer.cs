using EventBusRabbitMQ.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ.Producer
{
	public class EventBusRabbitMQProducer
	{
		private readonly IRabbitMQConnection _connection;

		public EventBusRabbitMQProducer(IRabbitMQConnection connection)
		{
			_connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		public void PublishBasketCheckout(string queueName, BasketCheckoutEvent publishModel)
		{
			using (var channel = _connection.CreateModel())
			{
				
				channel.QueueDeclare(
					queue: queueName,
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: null
					);
				var message = JsonConvert.SerializeObject(publishModel);
				var body = Encoding.UTF8.GetBytes(message);

				IBasicProperties properties = channel.CreateBasicProperties();
				properties.Persistent = true;
				properties.DeliveryMode = 2;

				// https://rabbitmq.com/tutorials/amqp-concepts.html
				channel.ConfirmSelect();
				channel.BasicPublish(
					exchange: "", // Direct exchg (empty str == amq.direct)
					routingKey: queueName, 
					mandatory: true,
					basicProperties: properties,
					body: body
					);
				channel.WaitForConfirmsOrDie();

				channel.BasicAcks += (sender, EventArgs) =>
				{
					Console.WriteLine("Sent msg acknowledge to RabbitMQ");
					//implement ack handle
				};
				channel.ConfirmSelect();



			}
		}
	}
}
