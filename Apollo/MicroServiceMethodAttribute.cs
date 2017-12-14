using System;

namespace Apollo
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MicroServiceMethodAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
