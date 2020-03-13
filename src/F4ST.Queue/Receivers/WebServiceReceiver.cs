using System.Collections.Generic;
using F4ST.Queue.QMessageModels.RequestMessages;

namespace F4ST.Queue.Receivers
{
    public abstract class WebServiceReceiver
    {
        public QWebRequestMessage Request { get; set; }
        public Dictionary<string, string[]> ResponseHeader = null;
    }
}