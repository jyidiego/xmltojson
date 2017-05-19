using System;
using APIService.Models;
using RabbitMQ.Client;

namespace APIService.Services
{
  public interface IQueueProducerService
  {
    IModel Model {get;set;}
    string QueueName {get;set;}
    string RoutingKeyName {get;set;}
    string ExchangeName {get;set;}
    string ExchangeType{get;set;}
    void WriteToQueue(string exchangeName, string queueName, string routingKeyName, string messsage);
    bool Connect(ConnectionFactory connectionFactory);
  }
}
