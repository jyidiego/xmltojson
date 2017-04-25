using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using APIService.Services;
using APIService.Repository;
using APIService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace APIService.Tests
{
    public class QueueConsumerServiceTests
    {
        private ILoggerFactory _loggerFactory;
        private IMetricsRepository _metricsRepo;
        private ConnectionFactory _connectionFactory;
        
        public QueueConsumerServiceTests()
        {
            var logger = new Mock<ILogger>();
            var loggerFactory = new Mock<ILoggerFactory>();          
            loggerFactory.Setup(c => c.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            _loggerFactory = loggerFactory.Object;

            var metricsRepo = new Mock<IMetricsRepository>();
            _metricsRepo = metricsRepo.Object;

            var model = new Mock<IModel>();
            var connection = new Mock<IConnection>();
            connection.Setup(c=>c.CreateModel()).Returns(model.Object);
            var connectonFactory = new Mock<ConnectionFactory>();
            connectonFactory.Setup(c=>c.CreateConnection()).Returns(connection.Object);
            _connectionFactory = connectonFactory.Object;
        }

        [Fact]
        public void CanCreateInstance()
        {            
            var queueConsumerService = new QueueConsumerService(_loggerFactory, _metricsRepo);
            Assert.NotNull(queueConsumerService);
        }

        [Fact]
        public void CanConnect()
        {            
            var queueConsumerService = new QueueConsumerService(_loggerFactory, _metricsRepo);
            Assert.NotNull(queueConsumerService);
            Assert.True(queueConsumerService.Connect(_connectionFactory));
        }

        [Fact]
        public void ReadFromQueue()
        {
            var modelMock = new Mock<IModel>();
            var connection = new Mock<IConnection>();

            modelMock.Setup(m=>m.QueueDeclare("QueueName",true,false,false,null)).Returns(new QueueDeclareOk("QueueName", 0, 1));
            var model = modelMock.Object;
            var consumer = new EventingBasicConsumer(model);
            var connectonFactory = new Mock<ConnectionFactory>();

            connection.Setup(c=>c.CreateModel()).Returns(model);            
            connectonFactory.Setup(c=>c.CreateConnection()).Returns(connection.Object); 
            
            var queueConsumerService = new QueueConsumerService(_loggerFactory, _metricsRepo);
            queueConsumerService.EventingBasicConsumer = consumer;
            Assert.NotNull(queueConsumerService);
            Assert.True(queueConsumerService.Connect(connectonFactory.Object));
            Action<string,QueueConsumerService,ulong,QueueMetric> onDequeue = (string message, QueueConsumerService service, ulong deliveryTag, QueueMetric queueMetric)=>{
                throw new Exception("whts up");
            };
            Action<Exception,QueueConsumerService,ulong,QueueMetric> onError = (Exception error, QueueConsumerService service, ulong deliveryTag, QueueMetric queueMetric)=>{
        
            };
            queueConsumerService.ReadFromQueue(onDequeue,onError,"ExchangeName","QueueName","RoutingKeyName");
            
           queueConsumerService.EventingBasicConsumer.HandleBasicDeliver("", It.IsAny<ulong>(), false, 
            "ExchangeName", "RoutingKeyName", null, Encoding.ASCII.GetBytes("hi there"));
        }
    }

    public class BasicDeliverEventArgsWrapper : BasicDeliverEventArgs
    {   
        public BasicDeliverEventArgsWrapper(string message) : base()
        {
            Body = Encoding.ASCII.GetBytes(message);           
        } 

         public BasicDeliverEventArgsWrapper(string consumerTag, ulong deliveryTag, bool redelivered, 
         string exchange, string routingKey, IBasicProperties properties, byte[] body):base(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body)
         {
             Body = body;             
         }
        public byte[] Body { get; set; } 
    }
}
