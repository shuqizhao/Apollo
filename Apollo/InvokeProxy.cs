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
            var channel = new Channel("127.0.0.1:5001", ChannelCredentials.Insecure);
            var client = new ApolloService.ApolloServiceClient(channel);
            var request = new Request();
            request.ServiceName = type.FullName + "$" + targetMethod.Name;
            var jsonInput = JsonConvert.SerializeObject(args);
            request.Data = jsonInput;
            var response = client.Call(request);
            channel.ShutdownAsync().Wait();
            var result = JsonHelper.DeserializeJsonToObject(response.Data,targetMethod.ReturnType);
            return result;
        }
    }
}