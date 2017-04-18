using System.Collections.Generic;
using System.Threading.Tasks;
using APIService.Models;

namespace APIService.Repository
{
    public interface IMetricsRepository
    {
        Task SaveMetric(QueueMetric metric);
        Task<IList<QueueMetric>> GetMetric(string queueName);
        Task<bool> ResetMetrics(string queueName);
       
    }
}