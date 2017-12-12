using System;
using System.Collections.Generic;
using Apollo;

namespace Apollo.DemoInterface
{
    [MicroService]
    public interface IValues
    {
       IEnumerable<string> Get();

       Person Hello();
    }
}
