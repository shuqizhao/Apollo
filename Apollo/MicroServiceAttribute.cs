using System;

namespace Apollo
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class MicroServiceAttribute : Attribute
    {
        public int Port { get; set; }

        public string Name { get; set; }
    }
}
