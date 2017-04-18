using System;
using APIService.Models;
using APIService.Repository;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace APIService.Services
{
  public class QueueService : IQueueService
  {
    private ConnectionFactory _connectionFactory;
    private IQueueConsumerService _queueConsumerService;
    private ILogger<QueueService> _logger; 
    public QueueService(IQueueConsumerService queueConsumerService, ConnectionFactory rabbitConnection, ILoggerFactory loggerFactory)
    {
       _connectionFactory = rabbitConnection;
      
      _queueConsumerService = queueConsumerService;
      _logger = loggerFactory.CreateLogger<QueueService>();

      _queueConsumerService.QueueName = "test-queue";
      _queueConsumerService.ExchangeName ="ExchangeName";
      _queueConsumerService.ExchangeType = "direct";
      _queueConsumerService.RoutingKeyName = string.Empty;
      _queueConsumerService.Connect(_connectionFactory);
      
    }

    public void ProcessMessage(string message, QueueConsumerService queueConsumerService, ulong deliveryTag, QueueMetric queueMetric)
    {
      queueConsumerService.Model.BasicAck(deliveryTag, false);
      queueMetric.RoutingAction = RoutingAction.Ignored;

      _logger.LogInformation($"Message recieved: {message}");
    }

    private void RaiseException(Exception ex, QueueConsumerService queueConsumerService, ulong deliveryTag, QueueMetric queueMetric)
    {
      queueConsumerService.Model.BasicNack(deliveryTag, false, false);
      queueMetric.RoutingAction = RoutingAction.Failed;

      _logger.LogError($"Error raised from QueueService: {ex.Message}");
    }

    public void ProcessQueue()
    {
      _queueConsumerService.ReadFromQueue(ProcessMessage, RaiseException,_queueConsumerService.ExchangeName,
        _queueConsumerService.QueueName,_queueConsumerService.RoutingKeyName);
    }

    

    
  }
}