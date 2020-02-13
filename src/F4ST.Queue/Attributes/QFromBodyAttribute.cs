using System;

namespace F4ST.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class QFromBodyAttribute : Attribute
    {
    }
}