using System;
using Apollo;

namespace Apollo.DemoInterface
{
    [MicroService]
    public interface IServerDemo
    {
        string Hello(string input);
    }
}
