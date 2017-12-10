using System;
using Apollo;

namespace Apollo.Console
{

    [MicroService(Port=5001)]
    public interface IServerDemo
    {
        string Hello(string input);
    }
}
