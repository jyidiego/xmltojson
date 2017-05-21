using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

using APIService.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

using DataFormatConverter;
using Newtonsoft.Json;

namespace APIService.Handlers
{
    public class XmlToJsonHandler : IMessageHandler
    {
        private ConnectionFactory _connectionFactory;
        private ILogger<XmlToJsonHandler> _logger;
        private IQueueProducerService _queueProducerService;
        private Converter _converter;

        public XmlToJsonHandler(IQueueProducerService queueProducerService, ConnectionFactory rabbitConnection, ILoggerFactory loggerFactory, IOptions<DataFlowServiceConfig> config)
        {
            _logger = loggerFactory.CreateLogger<XmlToJsonHandler>();
            _queueProducerService = queueProducerService;
            _connectionFactory = rabbitConnection;
            _queueProducerService.QueueName = config.Value.OutQueueName;
            _queueProducerService.ExchangeName = config.Value.OutExchangeName;
            _queueProducerService.ExchangeType = config.Value.ExchangeType;
            _queueProducerService.RoutingKeyName = config.Value.OutRoutingKeyName;
            _queueProducerService.Connect(_connectionFactory);
            _converter = new Converter();
        }

        public bool Handle(string message)
        {
            _logger.LogInformation($"XmlToJson Message Handler Message Length: {message.Length}");
            var tradelist = _converter.XML_to_TradeJson(message);
            _logger.LogInformation("XmlToJson Message Handler: Output");


            if (!tradelist.Equals(null))
            {
                foreach (var trade in tradelist)
                {
                    _logger.LogInformation($"trade: {trade}");
                    var tradeJson = JsonConvert.SerializeObject(trade);
                    _queueProducerService.WriteToQueue(_queueProducerService.ExchangeName,
                                        _queueProducerService.QueueName,
                                        _queueProducerService.RoutingKeyName,
                                        tradeJson);
                    _logger.LogInformation("XmlToJson Message Handler written to queue");
                }
                return true;
            }

            _logger.LogInformation($"Error Message: {message}");
            return false;

        }
    }
}
