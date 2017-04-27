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
using RabbitMQ.Client.Framing.Impl;
using APIService.Handlers;

namespace APIService.Tests
{
  public class QueueServiceTests
  {
    
     [Fact]
     public void CreateInstance()
        {
            var logger = new Mock<ILogger>();
            var loggerFactory = new Mock<ILoggerFactory>();          
            loggerFactory.Setup(c => c.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var metricsRepo = new Mock<IMetricsRepository>();
           
            var model = new Mock<IModel>();

            var connection = new Mock<IConnection>();
            connection.Setup(c=>c.CreateModel()).Returns(model.Object);

            var connectonFactory = new Mock<ConnectionFactory>();
            connectonFactory.Setup(c=>c.CreateConnection()).Returns(connection.Object);

            var queueConsumerService = new Mock<IQueueConsumerService>();
            
            var queueService = new QueueService(queueConsumerService.Object, connectonFactory.Object, loggerFactory.Object);
            Assert.NotNull(queueService);
        }

         [Fact]
     public void RegisterHandler()
        {
            var logger = new Mock<ILogger>();
            var loggerFactory = new Mock<ILoggerFactory>();          
            loggerFactory.Setup(c => c.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var metricsRepo = new Mock<IMetricsRepository>();
           
            var model = new Mock<IModel>();
            
            var connection = new Mock<IConnection>();
            connection.Setup(c=>c.CreateModel()).Returns(model.Object);

            var connectonFactory = new Mock<ConnectionFactory>();
            connectonFactory.Setup(c=>c.CreateConnection()).Returns(connection.Object);

            var queueConsumerService = new Mock<IQueueConsumerService>();
            
            var queueService = new QueueService(queueConsumerService.Object, connectonFactory.Object, loggerFactory.Object);
            Assert.NotNull(queueService);

            var handler = new Mock<IMessageHandler>();
            queueService.RegisterHandler(handler.Object);
      }

      public void ProcessMessage()
        {
            var logger = new Mock<ILogger>();
            var loggerFactory = new Mock<ILoggerFactory>();          
            loggerFactory.Setup(c => c.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            var metricsRepo = new Mock<IMetricsRepository>();
           
            var model = new Mock<IModel>();
            
            var connection = new Mock<IConnection>();
            connection.Setup(c=>c.CreateModel()).Returns(model.Object);

            var connectonFactory = new Mock<ConnectionFactory>();
            connectonFactory.Setup(c=>c.CreateConnection()).Returns(connection.Object);

            var queueConsumerService = new Mock<IQueueConsumerService>();
            
            var queueService = new QueueService(queueConsumerService.Object, connectonFactory.Object, loggerFactory.Object);
            Assert.NotNull(queueService);

            var handler = new Mock<IMessageHandler>();
            queueService.RegisterHandler(handler.Object);

            queueService.ProcessMessage("the message body",queueConsumerService.Object,It.IsAny<ulong>(),new QueueMetric());
      }
  }
}