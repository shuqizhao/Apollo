using System;
using System.Reflection;
using Grpc.Core;
using Newtonsoft.Json;

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
            try
            {
                var serviceKey = MicroServiceManage.BuildServiceKey(type);
                var methodKey = MicroServiceManage.BuildMethodKey(serviceKey, targetMethod);

                var response = MicroServiceManage.Call(serviceKey, methodKey, args);

                if (targetMethod.ReturnType == typeof(void))
                {
                    return "";
                }
                var result = JsonHelper.DeserializeJsonToObject(response.Data, targetMethod.ReturnType);
                return result;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            return null;
        }
    }
}