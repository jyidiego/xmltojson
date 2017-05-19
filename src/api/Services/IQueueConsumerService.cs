using System;
using APIService.Models;
using RabbitMQ.Client;

namespace APIService.Services
{
  public interface IQueueConsumerService
  {
    IModel Model {get;set;}
    string QueueName {get;set;}
    string RoutingKeyName {get;set;}
    string ExchangeName {get;set;}
    string ExchangeType{get;set;}
    void ReadFromQueue(Action<string, QueueConsumerService, ulong, QueueMetric> onDequeue, Action<Exception, QueueConsumerService, ulong, QueueMetric> onError, 
      string exchangeName, string queueName, string routingKeyName);
    bool Connect(ConnectionFactory connectionFactory);
  }
}