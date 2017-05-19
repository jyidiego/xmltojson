using System;

namespace APIService.Handlers
{
  public interface IMessageHandler
  {
    bool Handle (string message);
  }
}