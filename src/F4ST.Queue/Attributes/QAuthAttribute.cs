using System;

namespace F4ST.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class QAuthAttribute : Attribute
    {
    }
}