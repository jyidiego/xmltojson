using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using APIService.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace APIService.Repository
{
  public class RedisMetricsRepository : IMetricsRepository
  {
    private ILogger<RedisMetricsRepository> _logger;        
    private IConnectionMultiplexer _redis;

    public RedisMetricsRepository(IConnectionMultiplexer redis, ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<RedisMetricsRepository>();
      _redis = redis;
    }

    public async Task<IList<QueueMetric>> GetMetric(string queueName)
    {
      throw new NotImplementedException();
    }

    public async Task<IList<QueueMetric>> GetMetrics()
    {
      throw new NotImplementedException();
      
    }

    public async Task<bool> ResetMetrics(string queueName)
    {
      throw new NotImplementedException();
    }

    public async Task SaveMetric(QueueMetric metric)
    {
      var db = GetDatabase();
      await db.HashSetAsync("QueueMetric-" + metric.Id, metric.ToHashEntries());
    }
    private IDatabase GetDatabase()
    {
      return _redis.GetDatabase();
    }

    
   
  }
}