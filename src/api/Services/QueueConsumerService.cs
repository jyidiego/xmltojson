using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using APIService.Models;
using APIService.Repository;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Framing.Impl;

namespace APIService.Services
{
  public class QueueConsumerService : IQueueConsumerService
  {
    private ConnectionFactory _connectionFactory;
    private ILogger<QueueConsumerService> _logger; 
    private IMetricsRepository _metricsRepository;
    private IConnection _connection;       
    public QueueConsumerService(ILoggerFactory loggerFactory, IMetricsRepository metricsRepository)
    {
      _metricsRepository = metricsRepository;
      _logger = loggerFactory.CreateLogger<QueueConsumerService>();
      
    }

    public IModel Model {get;set;}
    public string QueueName {get;set;}
    public string RoutingKeyName {get;set;}
    public string ExchangeName {get;set;}
    public string ExchangeType{get;set;}
    public EventingBasicConsumer EventingBasicConsumer {get;set;}



    /// <summary>
    /// Read a message from the queue.
    /// </summary>
    /// <param name="onDequeue">The action to take when recieving a message</param>
    /// <param name="onError">If an error occurs, provide an action to take.</param>
    /// <param name="exchangeName">Name of the exchange.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="routingKeyName">Name of the routing key.</param>
    public void ReadFromQueue(Action<string, QueueConsumerService, ulong, QueueMetric> onDequeue, Action<Exception, QueueConsumerService, ulong, QueueMetric> onError, 
        string exchangeName, string queueName, string routingKeyName)
    {
        BindToQueue(exchangeName, queueName, routingKeyName);
        
        if(this.EventingBasicConsumer == null)
            EventingBasicConsumer = new EventingBasicConsumer(Model);
          
        // Receive the message from the queue and act on that message
        EventingBasicConsumer.Received += (o, e) =>
            {
                _logger.LogInformation("Messge Received handler invoked.");

                var queueMetric = new QueueMetric
                {
                        QueueName = this.QueueName,
                        ExchangeName = this.ExchangeName,
                        RoutingKeyName = this.RoutingKeyName,
                        ConsumedDateTime = DateTime.UtcNow,
                        InstanceId = GetInstanceId(),                        
                };

                try
                {
                    var queuedMessage = Encoding.ASCII.GetString(e.Body);
                    queueMetric.MessageLength = queuedMessage.Length;                    

                    onDequeue.Invoke(queuedMessage, this, e.DeliveryTag, queueMetric);
                    _metricsRepository.SaveMetric(queueMetric).GetAwaiter();
                }
                catch(Exception ex)
                {
                    onError.Invoke(ex,this, e.DeliveryTag, queueMetric);
                    _metricsRepository.SaveMetric(queueMetric).GetAwaiter();
                }
               

            };

       
 
        // If the consumer shutdowns reconnect to rabbit and begin reading from the queue again.
        EventingBasicConsumer.Shutdown += (o, e) =>
            {
                Connect();
                ReadFromQueue(onDequeue, onError, exchangeName, queueName, routingKeyName);
            };
 
        Model.BasicConsume(queueName, false, EventingBasicConsumer);
    }

    private string GetInstanceId()
    {
      return Guid.NewGuid().ToString();
    }

   



    /// <summary>
    /// Bind to a queue.
    /// </summary>
    /// <param name="exchangeName">Name of the exchange.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="routingKeyName">Name of the routing key.</param>
    private void BindToQueue(string exchangeName, string queueName, string routingKeyName)
    {
        const bool durable = true, autoDelete = false, exclusive = false;
 
        Model.BasicQos(0, 1, false);
 
        // replicate the queue to all hosts. Queue arguments are optional
        var queueArgs = new Dictionary<string, object>
                {
                    {"x-ha-policy", "all"}
                };
        var queueDeclareOk = Model.QueueDeclare(queueName, durable, exclusive, autoDelete, queueArgs);

        if(queueDeclareOk != null)
            QueueName = queueDeclareOk.QueueName;

        Model.QueueBind(queueName, exchangeName, routingKeyName, null);
    }

    /// <summary>
    /// Connect to rabbit mq.
    /// </summary>
    /// <returns><c>true</c> if a connection to RabbitMQ has been made, <c>false</c> otherwise</returns>

    public bool Connect()
    {
        return Connect(_connectionFactory);
    }


    /// <summary>
    /// Connect to rabbit mq.
    /// </summary>
    /// <returns><c>true</c> if a connection to RabbitMQ has been made, <c>false</c> otherwise</returns>
    public bool Connect(ConnectionFactory connectionFactory)
    {
        if(_connectionFactory == null)
            _connectionFactory= connectionFactory;

        int attempts = 0;
        // make 3 attempts to connect to RabbitMQ just in case an interruption occurs during the connection
        while (attempts < 3)
        {
            attempts++;
 
            try
            {
                _logger.LogInformation($"Connecting to Rabbit, Attempt #{attempts+1} of 3");
                _connection = connectionFactory.CreateConnection();
                _logger.LogInformation($"Connected");
 
                // Create the model 
                CreateModel();
 
                return true;
            }
            catch (System.IO.EndOfStreamException ex)
            {
                _logger.LogError($"End of Stream Exception creating Rabbit connection {ex.ToString()}");
                return false;
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError($"Broker Unreachable when creating Rabbit connection {ex.ToString()}");
                return false;
            }
 
            // wait before trying again
            Thread.Sleep(1000);
        }
 
        if (_connection != null)
            _connection.Dispose();
 
        return false;
    }
 
    /// <summary>
    /// Create a model.
    /// </summary>
    private void CreateModel()
    {
        Model = _connection.CreateModel();
 
        // When AutoClose is true, the last channel to close will also cause the connection to close. 
        // If it is set to true before any channel is created, the connection will close then and there.
        _connection.AutoClose = true;
 
        // Configure the Quality of service for the model. Below is how what each setting means.
        // BasicQos(0="Dont send me a new message untill I’ve finshed",  1= "Send me one message at a time", false ="Apply to this Model only")
        Model.BasicQos(0, 1, false);
 
        const bool durable = true, exchangeAutoDelete = false, queueAutoDelete = false, exclusive = false;
 
        // Create a new, durable exchange, and have it auto delete itself as long as an exchange name has been provided.
        if (!string.IsNullOrWhiteSpace(ExchangeName))
            Model.ExchangeDeclare(ExchangeName, ExchangeType, durable, exchangeAutoDelete, null);
 
        // replicate the queue to all hosts. Queue arguments are optional
        var queueArgs = new Dictionary<string, object>
                {
                    {"x-ha-policy", "all"}
                };
        Model.QueueDeclare(QueueName, durable, exclusive, queueAutoDelete, queueArgs);
        Model.QueueBind(QueueName, ExchangeName, RoutingKeyName ?? string.Empty, null);
    }

   
  }
}