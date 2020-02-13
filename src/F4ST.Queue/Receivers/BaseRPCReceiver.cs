using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using F4ST.Common.Containers;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;
using F4ST.Queue.QMessageModels.SendMessages;

namespace F4ST.Queue.Receivers
{
    public class BaseRPCReceiver<TReceiver> : Receiver<QClassRequestMessage, QClassResponseMessage, QClassMessage>
        where TReceiver : RPCReceiver
    {
        //protected override string QueueName => GetQueueAttributeName() ?? GetType().Name;
        protected override bool HaveRequestMessage => true;
        protected override bool HaveSendMessage => true;


        public BaseRPCReceiver(QSettingModel qSetting) : base(qSetting)
        {
        }

        public static string GetCurrentNamespace()
        {
            return new StackFrame(2)?.GetMethod().DeclaringType?.Namespace ?? "";
        }

        private MethodInfo FindMethod(TReceiver rcInstance, string targetMethod)
        {
            var methods = rcInstance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var cMethods = methods.Where(m => m.Name == targetMethod).ToArray();
            return cMethods.FirstOrDefault();
        }

        protected override async Task<QClassResponseMessage> ProcessRequestMessage(QClassRequestMessage request)
        {
            var wr = IoC.Resolve<TReceiver>();
            var method = FindMethod(wr, request.MethodName);

            if (!string.IsNullOrWhiteSpace(request.Lang))
            {
                var cultureInfo = new CultureInfo(request.Lang);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }

            var res = Globals.RunMethod(wr, method, request.Parameters, true);
            return new QClassResponseMessage()
            {
                Result = res
            };
        }

        protected override async Task ProcessSendMessage(QClassMessage request)
        {
            var wr = IoC.Resolve<TReceiver>();
            var method = FindMethod(wr, request.MethodName);

            if (!string.IsNullOrWhiteSpace(request.Lang))
            {
                var cultureInfo = new CultureInfo(request.Lang);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }

            Globals.RunMethod(wr, method, request.Parameters, false);
        }

        public void Start()
        {
        }

    }
}