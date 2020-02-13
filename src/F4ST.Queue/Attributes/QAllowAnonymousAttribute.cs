using System;

namespace F4ST.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class QAllowAnonymousAttribute : Attribute
    {
    }
}