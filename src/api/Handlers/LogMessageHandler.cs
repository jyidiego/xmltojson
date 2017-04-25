using System;
using Microsoft.Extensions.Logging;

namespace APIService.Handlers
{
  public class LogMessageHandler : IMessageHandler
  {
    private ILogger<LogMessageHandler> _logger; 
    public LogMessageHandler(ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<LogMessageHandler>();
    }

    public bool Handle(string message)
    {
      _logger.LogInformation($"Log Message Handler: {message}");
      return true;
    }
  }
}