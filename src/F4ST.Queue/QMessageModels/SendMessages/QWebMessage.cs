using System.Collections.Generic;
using System.Linq;

namespace F4ST.Queue.QMessageModels.SendMessages
{
    public class QWebMessage : QBaseMessage, IQWebMessage
    {
        private string _arguments;
        private string _queryStrings;

        /// <inheritdoc />
        public string TraceId { get; set; }

        /// <inheritdoc />
        public string Scheme { get; set; }

        /// <inheritdoc />
        public string Domain { get; set; }

        public string ContentType { get; set; }

        /// <inheritdoc />
        public string BasePath { get; set; }

        /// <inheritdoc />
        public string BaseUrl => $"{Scheme}://{Domain}{BasePath}/";

        public string Target => Url.Split('/').FirstOrDefault();

        /// <inheritdoc />
        public string Url { get; set; }

        /// <inheritdoc />
        public string IP { get; set; }

        /// <inheritdoc />
        public string HttpMethod { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string[]> Headers { get; set; }

        /// <inheritdoc />
        public string Arguments { get; set; }

        /// <inheritdoc />
        public string QueryStrings { get; set; }

        /// <inheritdoc />
        public byte[] Body { get; set; }

        /// <inheritdoc />
        public string Lang { get; set; }

        /// <inheritdoc />
        public virtual bool IsAuthenticated { get; }

    }
}