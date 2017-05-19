using System;

namespace APIService.Services
{
	public class DataFlowServiceConfig
	{
		public DataFlowServiceConfig()
		{ }

		public string InQueueName { get; set; }

		public string OutQueueName { get; set; }

		public string InExchangeName { get; set; }

        public string OutExchangeName { get; set; }

		public string ExchangeType { get; set; }

		public string InRoutingKeyName { get; set; }

		public string OutRoutingKeyName { get; set; }

	}
}
