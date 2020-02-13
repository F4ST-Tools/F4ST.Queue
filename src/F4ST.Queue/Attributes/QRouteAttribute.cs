using System;

namespace F4ST.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QRouteAttribute : Attribute
    {
        public string Route { get; }

        public QRouteAttribute(string route)
        {
            Route = route;
        }
    }
}