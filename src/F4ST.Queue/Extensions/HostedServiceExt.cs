using Castle.MicroKernel.Registration;
using Castle.Windsor;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.Receivers;

namespace F4ST.Queue.Extensions
{
    public static class HostedServiceExt
    {
        public static void AddWebReceiverService<THostedService>(this WindsorContainer container, QSettingModel qSetting)
            where THostedService : WebServiceReceiver
        {
            var wr = new BaseWebServiceReceiver<THostedService>(qSetting);
            wr.Start();

            container.Register(Component.For<THostedService>());
            
        }

        public static void AddRpcReceiverService<THostedService>(this WindsorContainer container, QSettingModel qSetting)
            where THostedService : RPCReceiver
        {
            var wr = new BaseRPCReceiver<THostedService>(qSetting);
            wr.Start();

            container.Register(Component.For<THostedService>());
            
        }

        public static void AddWebReceiverService(this IWindsorContainer container, QSettingModel qSetting)
        {
            var wr = new BaseWebReceiver(qSetting);
            wr.Start();
        }
    }
}