using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StackExchange.Redis;

namespace APIService.Models
{
  public class QueueMetric : Repository.IRedisHashEntryable
  {
    public string Id {get;} = Guid.NewGuid().ToString();
    public string QueueName {get;set;}

    public string ExchangeName  {get;set;}
    
    public string RoutingKeyName {get;set;}

    public string InstanceId {get;set;}

    public DateTime ReceivedDateTime {get;set;}

    public DateTime ConsumedDateTime {get;set;}

    public int MessageLength {get;set;}

    public RoutingAction RoutingAction {get;set;}

    public HashEntry[] ToHashEntries()
    {
       var entries = new List<HashEntry>();
       entries.Add(new HashEntry("InstanceId", this.InstanceId));
       entries.Add(new HashEntry("QueueName", this.QueueName));
       entries.Add(new HashEntry("ExchangeName", this.ExchangeName));
       entries.Add(new HashEntry("RoutingKeyName", this.RoutingKeyName));
       entries.Add(new HashEntry("ConsumedDateTime", this.ConsumedDateTime.ToString()));
       entries.Add(new HashEntry("ReceivedDateTime", this.ReceivedDateTime.ToString()));
       entries.Add(new HashEntry("MessageLength", this.MessageLength));
       entries.Add(new HashEntry("RoutingAction", (int)this.RoutingAction));
       return entries.ToArray();
    }

  }

  public enum RoutingAction
  {
    Ignored,
    Failed
  }
}