using System.Net.Http;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;

namespace F4ST.Queue.Receivers
{
    public interface IWebReceiverExt
    {
        void BeforeSend(HttpRequestMessage httpMessage, QWebRequestMessage request);
    }
}