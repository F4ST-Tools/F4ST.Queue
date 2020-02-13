using System;

namespace F4ST.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class QFromHeaderAttribute : Attribute
    {
    }
}