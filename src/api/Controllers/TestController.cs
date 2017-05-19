using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace apiservice.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private ConnectionFactory _connectionFactory;
        private ILogger<TestController> _logger;
        public TestController(ConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
             _connectionFactory = connectionFactory;
             _logger = loggerFactory.CreateLogger<TestController>();
        }

        // POST api/test
        [HttpPost]
        public void Post(string message)
        {
            if (string.IsNullOrEmpty(message) )
                throw new Exception("no data found");

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                CreateQueue(channel);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "ExchangeName",
                                        routingKey: "",
                                        basicProperties: null,
                                        body: body);

                _logger.LogInformation("Message published to RabbitMQ");
                
            }
            
          
        }

        protected void CreateQueue(IModel channel)
        {
            channel.QueueDeclare(queue: "test-queue",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        }

    }
}
