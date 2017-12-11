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
            var service = ConsulHelper.GetServer(type.FullName);
            var channel = new Channel(service.Address + ":" + service.Port, ChannelCredentials.Insecure);
            System.Console.WriteLine(service.Address + ":" + service.Port);
            var client = new ApolloService.ApolloServiceClient(channel);
            var request = new Request();
            request.ServiceName = type.FullName + "$" + targetMethod.Name;
            var jsonInput = JsonConvert.SerializeObject(args);
            request.Data = jsonInput;
            var response = client.Call(request);
            channel.ShutdownAsync().Wait();
            var result = JsonHelper.DeserializeJsonToObject(response.Data, targetMethod.ReturnType);
            return result;
        }
    }
}