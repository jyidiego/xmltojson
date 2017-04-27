using System.Collections.Generic;
using APIService.Handlers;
using APIService.Models;

namespace APIService.Services
{
  public interface IQueueService
  {

    void ProcessQueue();

    void ProcessMessage(string message, IQueueConsumerService queueConsumerService, ulong deliveryTag, QueueMetric queueMetric);

    void RegisterHandler(IMessageHandler handler);

    void RegisterHandlers(IEnumerable<IMessageHandler> handlers);
    
  }
}