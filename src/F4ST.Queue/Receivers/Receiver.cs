using System;
using System.Threading.Tasks;
using EasyNetQ;
using F4ST.Common.Containers;
using F4ST.Common.Tools;
using F4ST.Queue.Extensions;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;

namespace F4ST.Queue.Receivers
{
    public abstract class Receiver<TRequest, TResponse, TMessage> : IDisposable
        where TRequest : QBaseRequest
        where TResponse : QBaseResponse
        where TMessage : QBaseMessage
    {
        protected abstract Task<TResponse> ProcessRequestMessage(TRequest request);
        protected abstract Task ProcessSendMessage(TMessage request);

        protected abstract bool HaveRequestMessage { get; }
        protected abstract bool HaveSendMessage { get; }

        protected QSettingModel SettingModel { get; }

        private IBus _bus;

        protected Receiver(QSettingModel qSetting)
        {
            SettingModel = qSetting;

            var connectionString = QCreateConnectionString.CreateConnection(qSetting, false);
            _bus = RabbitHutch.CreateBus(connectionString);

            if (HaveRequestMessage)
            {
                _bus.RespondAsync<TRequest, TResponse>(HandleRequestMessage,
                    c => c.WithQueueName(qSetting.QueueName));
            }

            if (HaveSendMessage)
            {
                _bus.SubscribeAsync<TMessage>(
                    $"{qSetting.QueueName}_R",
                    HandleSendMessage,
                    c => c.WithQueueName($"{qSetting.QueueName}_R"));
            }
        }

        private async Task<TResponse> HandleRequestMessage(TRequest request)
        {
            var res = await ProcessRequestMessage(request);
            return res;
        }

        private async Task HandleSendMessage(TMessage request)
        {
            await ProcessSendMessage(request);
        }

        public void Dispose()
        {
            _bus?.Dispose();
            _bus = null;
        }
    }

    public abstract class Receiver : Receiver<QBaseRequest, QBaseResponse, QBaseMessage>
    {
        protected Receiver(QSettingModel qSetting) : base(qSetting)
        {

        }
    }
}