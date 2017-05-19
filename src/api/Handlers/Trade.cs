using System.Collections.Generic;

namespace APIService.Handlers
{
    class Trade
    {
        public string @OUTPUT_LIMIT { get; set; }

        public string @TRADED_SEC_EXPORT_ID { get; set; }

        public string @OUTPUT_TYPE { get; set; }

        public string @part { get; set; }

        public IList<string> Row { get; set; }
    }
}