using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using F4ST.Common.Containers;
using F4ST.Common.Extensions;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;
using F4ST.Queue.QMessageModels.SendMessages;
using Newtonsoft.Json;

namespace F4ST.Queue.Transmitters
{
    public class RPCTransmitter<T> : BaseProxy<T, RPCTransmitter<T>>
        where T : class
    {
        private QSettingModel qSetting;

        public static T Register(QSettingModel setting)
        {
            var res = CreateProxy(null);
            (res as RPCTransmitter<T>)?.SetQueueName(setting);

            return res;
        }

        private void SetQueueName(QSettingModel setting)
        {
            qSetting = setting;
        }

        protected override async Task<bool> BeforeRunMethod(MethodInfo targetMethod, object[] args)
        {
            await base.BeforeRunMethod(targetMethod, args);

            try
            {
                using (var transmitter = IoC.Resolve<ITransmitter>())
                {
                    if (targetMethod.ReturnType == typeof(void))
                    {
                        await transmitter.Send(qSetting, new QClassMessage()
                        {
                            Lang = CultureInfo.CurrentCulture.Name,
                            MethodName = targetMethod.Name,
                            Parameters = args
                        });
                    }
                    else
                    {
                        var res = await transmitter.Request(qSetting, new QClassRequestMessage()
                        {
                            Lang = CultureInfo.CurrentCulture.Name,
                            MethodName = targetMethod.Name,
                            Parameters = args
                        });
                        Debugger.Break();

                        if (res == null)
                        {
                            Result = null;
                            return false;
                        }

                        if (!(res is QClassResponseMessage response))
                        {
                            Result = null;
                            return false;
                        }

                        Result = ((string)response.Result).ToObject<string>();

                        if (!IsAsyncMethod(targetMethod))
                            return false;

                        if (targetMethod.ReturnType == typeof(Task))
                        {
                            Result = GetTaskResult();
                            return false;
                        }

                        var m = GetType().GetMethod("GetGenericResult");
                        var g = m?.MakeGenericMethod(targetMethod.ReturnType.GenericTypeArguments[0]);
                        Result = JsonConvert.DeserializeObject((string) Result, targetMethod.ReturnType.GenericTypeArguments[0]);
                        Result = g?.Invoke(this, new[] {Result});
                    }
                }
            }
            catch (Exception e)
            {
                Debugger.Log(1,"F4St.Queue", $"Error=>{e.Message}");
                throw;
            }

            return false;
        }

        public async Task<TT> GetGenericResult<TT>(TT result)
        {
            return result;
        }

        private async Task GetTaskResult()
        {
        }

    }
}