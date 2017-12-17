using System;
using System.Reflection;
using Grpc.Core;
using Newtonsoft.Json;

#if NET45
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
#endif

namespace Apollo
{
#if NETSTANDARD2_0
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
#endif

#if NET45
    public class InvokeProxy<T> : RealProxy
{
         private Type type = null;
         public InvokeProxy() : this(typeof(T))
         {
             type = typeof(T);
         }

        protected InvokeProxy(Type classToProxy) : base(classToProxy)
         {
         }

        public override IMessage Invoke(IMessage msg)
         {
            IMethodCallMessage callMessage = (IMethodCallMessage)msg;
            var targetMethod = (MethodInfo)callMessage.MethodBase;
            var serviceKey = MicroServiceManage.BuildServiceKey(type);
            var methodKey = MicroServiceManage.BuildMethodKey(serviceKey, targetMethod);

            var response = MicroServiceManage.Call(serviceKey, methodKey, callMessage.Args);

            var result = JsonHelper.DeserializeJsonToObject(response.Data, targetMethod.ReturnType);
            if (targetMethod.ReturnType == typeof(void))
            {
                result = "";
            }
            ReturnMessage message = new ReturnMessage(result,null,0,null,(IMethodCallMessage)msg);
            return (IMessage)message;
         }
}
#endif
}