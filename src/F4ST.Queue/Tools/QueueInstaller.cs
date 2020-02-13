using Castle.Windsor;
using F4ST.Common.Containers;
using F4ST.Common.Mappers;

namespace F4ST.Queue.Tools
{
    public class QueueInstaller : IIoCInstaller
    {
        public int Priority => -79;
        public void Install(WindsorContainer container, IMapper mapper)
        {

        }

    }
}