using APIService.Models;

namespace APIService.Services
{
  public interface IQueueService
  {

    void ProcessQueue();

    void ProcessMessage(string message, QueueConsumerService queueConsumerService, ulong deliveryTag, QueueMetric queueMetric);

  }
}