using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;

namespace Apollo
{
    class ApolloServiceImpl : ApolloService.ApolloServiceBase
    {
        public override Task<Response> Call(Request request, ServerCallContext context)
        {
            var splits = request.ServiceName.Split('$');
            var typeName = splits[0];
            var methodName = splits[1];
            var type = MicroServiceManage.GetServiceType(typeName);
            var methodType = type.GetMethod(methodName);
            var instance = Activator.CreateInstance(type);
            var args = JsonHelper.DeserializeJsonToObject<object[]>(request.Data);

            var result = methodType.Invoke(instance, args);
            //System.Console.WriteLine("I get message :"+request.ServiceName);
            var resultJson = JsonConvert.SerializeObject(result);
            return Task.FromResult(new Response { Data = resultJson });
        }
    }
}