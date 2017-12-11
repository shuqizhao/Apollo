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
                var service = ConsulHelper.GetServer(type.FullName);
                var channel = new Channel(service.Address + ":" + service.Port, ChannelCredentials.Insecure);
                //System.Console.WriteLine(service.Address + ":" + service.Port);
                var client = new ApolloService.ApolloServiceClient(channel);
                var request = new Request();

                var key = type.FullName + "_" + targetMethod.Name;
                foreach (var parameter in targetMethod.GetParameters())
                {
                    key += "-" + parameter.ParameterType.FullName;
                }

                request.ServiceName = type.FullName + "$" + key;
                var jsonInput = "";
                foreach (var arg in args)
                {
                    jsonInput += JsonConvert.SerializeObject(arg) + "兲";
                }
                request.Data = jsonInput;
                var response = client.Call(request);
                channel.ShutdownAsync().Wait();
                if(targetMethod.ReturnType == typeof(void)){
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