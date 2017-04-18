using APIService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace APIService.Extensions
{
  public static class QueueServiceExtensions
{
    public static void UseQueueService(this IApplicationBuilder app)
    {
        var queueService = app.ApplicationServices.GetRequiredService<IQueueService>();
        queueService.ProcessQueue();
    }
}
}