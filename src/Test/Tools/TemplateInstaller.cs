using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using F4ST.Common.Containers;
using F4ST.Common.Mappers;
using F4ST.Common.Tools;
using F4ST.Queue.Extensions;
using F4ST.Queue.Transmitters;
using Test.Controllers;
using Test.Models;

namespace Test.Tools
{
    public class TemplateInstaller : IIoCInstaller
    {
        public int Priority => 10;

        public void Install(WindsorContainer container, IMapper mapper)
        {
            //container.Register(Component.For<IInitProject>().ImplementedBy<InitProjectImp>().LifestyleSingleton());

            var appSetting = IoC.Resolve<IAppSetting>();

            var item = appSetting.GetQSetting("RPCTestClass");
            container.AddRpcReceiverService<RpcTestReceiver>(item);


            var test = RPCTransmitter<ITestClass>.Register(appSetting.GetQSetting("RPCTestClass"));
            container.Register(Component.For<ITestClass>().Instance(test).LifestyleSingleton());

        }

    }
}