using System;
using Apollo.DemoInterface;

namespace Apollo.DemoService
{

    public class ServerDemo : IServerDemo
    {
        public string Hello(string input)
        {
            System.Console.WriteLine("you input is "+input);
            return "I get it，乱码？";
        }
    }
}
