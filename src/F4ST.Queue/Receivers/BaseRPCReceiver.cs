using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using F4ST.Common.Containers;
using F4ST.Common.Extensions;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;
using F4ST.Queue.QMessageModels.SendMessages;
using Newtonsoft.Json;

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
            try
            {
                var wr = IoC.Resolve<TReceiver>();
                var method = FindMethod(wr, request.MethodName);

                if (!string.IsNullOrWhiteSpace(request.Lang))
                {
                    var cultureInfo = new CultureInfo(request.Lang);
                    CultureInfo.CurrentCulture = cultureInfo;
                    CultureInfo.CurrentUICulture = cultureInfo;
                }

                var parameters = ProcessParameters(method, request.Parameters);

                var res = Globals.RunMethod(wr, method, parameters, true);
                return new QClassResponseMessage()
                {
                    Result = JsonConvert.SerializeObject(res).ToBase64()
                };
            }
            catch (Exception e)
            {
                Debugger.Log(1,"F4ST.Queue", $"Error=>{e.Message}");
                throw;
            }
        }

        protected override async Task ProcessSendMessage(QClassMessage request)
        {
            var wr = IoC.Resolve<TReceiver>();
            var method = FindMethod(wr, request.MethodName);

            if (!string.IsNullOrWhiteSpace(request.Lang))
            {
                var cultureInfo = new CultureInfo(request.Lang);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            }

            var parameters = ProcessParameters(method, request.Parameters);
            Globals.RunMethod(wr, method, parameters, false);
        }

        private object[] ProcessParameters(MethodInfo method, object[] parameters)
        {
            var res=new List<object>();
            for (var i = 0; i < parameters.Length; i++)
            {
                var item = method.GetParameters()[i];
                res.Add(Convert.ChangeType(parameters[i], item.ParameterType));
            }

            return res.ToArray();
        }

        public void Start()
        {
        }

    }
}