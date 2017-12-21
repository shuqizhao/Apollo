using System;
using System.Reflection;
using System.Threading;
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
            object result = null;
            var needTry = false;
            var tryCount = 0;
            do
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
                    if (response.Code == "200")
                    {
                        result = JsonHelper.DeserializeJsonToObject(response.Data, targetMethod.ReturnType);
                    }
                    else if (response.Code == "500")
                    {
                        throw new Exception(response.Message);
                    }
                    needTry = false;
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex);
                    needTry = true;
                    if (tryCount == 3)
                    {
                        throw ex;
                    }
                }
                tryCount++;
                Thread.Sleep(5000);
            }
            while (needTry && tryCount < 3);

            return result;
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
            object result = null;
            var needTry = false;
            var tryCount = 0;
            do
            {
                try
                {
                    IMethodCallMessage callMessage = (IMethodCallMessage)msg;
                    var targetMethod = (MethodInfo)callMessage.MethodBase;
                    var serviceKey = MicroServiceManage.BuildServiceKey(type);
                    var methodKey = MicroServiceManage.BuildMethodKey(serviceKey, targetMethod);

                    var response = MicroServiceManage.Call(serviceKey, methodKey, callMessage.Args);
                    if (response.Code == "200")
                    {
                        result = JsonHelper.DeserializeJsonToObject(response.Data, targetMethod.ReturnType);
                    }
                    else if(response.Code == "500"){
                        throw new Exception(response.Message);
                    }
                    if (targetMethod.ReturnType == typeof(void))
                    {
                        result = "";
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex);
                    needTry = true;
                    if(tryCount == 3){
                        throw ex;
                    }
                }
                tryCount++;
                Thread.Sleep(5000);
            }
            while (needTry&&tryCount<3);
            ReturnMessage message = new ReturnMessage(result,null,0,null,(IMethodCallMessage)msg);
            return (IMessage)message;
         }
}
#endif
}