using APIService.Handlers;
using APIService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace APIService.Extensions
{
  public static class QueueServiceExtensions
{
    public static void StartQueueService(this IApplicationBuilder app)
    {
        var handlers = app.ApplicationServices.GetServices<IMessageHandler>();
        var queueService = app.ApplicationServices.GetRequiredService<IQueueService>();

        if(handlers != null)
            queueService.RegisterHandlers(handlers);
        
        if(!queueService.IsProcessing())
        {
             queueService.ProcessQueue();
        }
    
    }
}
}
