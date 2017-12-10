using System;
using System.Reflection;
namespace Apollo
{
    public class InvokeProxy<T> : DispatchProxy
    {
        private Type type = null;
        public InvokeProxy()
        {
            type = typeof(T);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            
            Console.WriteLine("Invoke 远程服务调用！");
            foreach(var arg in args){
                System.Console.WriteLine(arg);
            }
            return type.Name+"->"+targetMethod.Name;
        }
    }
}