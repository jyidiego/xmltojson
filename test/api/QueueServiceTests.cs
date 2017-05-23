using System;
using System.IO;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
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
			connection.Setup(c => c.CreateModel()).Returns(model.Object);

			var connectonFactory = new Mock<ConnectionFactory>();
			connectonFactory.Setup(c => c.CreateConnection()).Returns(connection.Object);

			var queueConsumerService = new Mock<IQueueConsumerService>();

			var appsettings = @"
{
  'Logging': {
    'IncludeScopes': false,
    'LogLevel': {
      'Default': 'Information'
    }
  },
  'redis': {
    'client': {
      'resolveDns': true

    }
  },
  'DataFlowServiceConfig': {
    'QueueName': 'IngestDB.FileSrc',
    'ExchangeName': 'ExchangeName',
    'ExchangeType': 'direct',
    'RoutingKeyName': ''
  }
}";

			var path = TestHelpers.CreateTempFile(appsettings);
			string directory = Path.GetDirectoryName(path);
			string fileName = Path.GetFileName(path);


			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.SetBasePath(directory);
			configurationBuilder.AddJsonFile(fileName);

			var config = configurationBuilder.Build();

			IOptions<DataFlowServiceConfig> iconfig = Options.Create<DataFlowServiceConfig>(new DataFlowServiceConfig());

			var queueService = new QueueService(queueConsumerService.Object, connectonFactory.Object, loggerFactory.Object, iconfig);
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
			connection.Setup(c => c.CreateModel()).Returns(model.Object);

			var connectonFactory = new Mock<ConnectionFactory>();
			connectonFactory.Setup(c => c.CreateConnection()).Returns(connection.Object);

			var queueConsumerService = new Mock<IQueueConsumerService>();

			var appsettings = @"
{
  'Logging': {
    'IncludeScopes': false,
    'LogLevel': {
      'Default': 'Information'
    }
  },
  'redis': {
    'client': {
      'resolveDns': true

    }
  },
  'DataFlowServiceConfig': {
    'QueueName': 'IngestDB.FileSrc',
    'ExchangeName': 'ExchangeName',
    'ExchangeType': 'direct',
    'RoutingKeyName': ''
  }
}";

			var path = TestHelpers.CreateTempFile(appsettings);
			string directory = Path.GetDirectoryName(path);
			string fileName = Path.GetFileName(path);


			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.SetBasePath(directory);
			configurationBuilder.AddJsonFile(fileName);

			var config = configurationBuilder.Build();

			IOptions<DataFlowServiceConfig> iconfig = Options.Create<DataFlowServiceConfig>(new DataFlowServiceConfig());


			var queueService = new QueueService(queueConsumerService.Object, connectonFactory.Object, loggerFactory.Object, iconfig);
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
			connection.Setup(c => c.CreateModel()).Returns(model.Object);

			var connectonFactory = new Mock<ConnectionFactory>();
			connectonFactory.Setup(c => c.CreateConnection()).Returns(connection.Object);

			var queueConsumerService = new Mock<IQueueConsumerService>();

			var appsettings = @"
{
  'Logging': {
    'IncludeScopes': false,
    'LogLevel': {
      'Default': 'Information'
    }
  },
  'redis': {
    'client': {
      'resolveDns': true

    }
  },
  'DataFlowServiceConfig': {
    'QueueName': 'IngestDB.FileSrc',
    'ExchangeName': 'ExchangeName',
    'ExchangeType': 'direct',
    'RoutingKeyName': ''
  }
}";

			var path = TestHelpers.CreateTempFile(appsettings);
			string directory = Path.GetDirectoryName(path);
			string fileName = Path.GetFileName(path);


			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.SetBasePath(directory);
			configurationBuilder.AddJsonFile(fileName);

			var config = configurationBuilder.Build();

			IOptions<DataFlowServiceConfig> iconfig = Options.Create<DataFlowServiceConfig>(new DataFlowServiceConfig());


			var queueService = new QueueService(queueConsumerService.Object, connectonFactory.Object, loggerFactory.Object, iconfig);
			Assert.NotNull(queueService);

			var handler = new Mock<IMessageHandler>();
			queueService.RegisterHandler(handler.Object);

			queueService.ProcessMessage("the message body", queueConsumerService.Object, It.IsAny<ulong>(), new QueueMetric());
		}
	}
}
