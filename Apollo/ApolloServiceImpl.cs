using System;
using System.Collections.Generic;
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
            try
            {
                var splits = request.ServiceName.Split('$');
                var typeName = splits[0];
                var methodName = splits[1];
                var type = MicroServiceManage.GetServiceType(typeName);
                var methodType = type.GetMethod(methodName);
                var parameters = methodType.GetParameters();
                var values = request.Data.Split('兲');
                var args = new List<object>();
                var i = 0;
                foreach (var parameter in parameters)
                {
                    var parameterType = parameter.ParameterType;
                    var value = JsonHelper.DeserializeJsonToObject(values[i], parameterType);
                    args.Add(value);
                    i++;
                }
                var instance = Activator.CreateInstance(type);
                //var args = JsonHelper.DeserializeJsonToObject<object[]>(request.Data);
                var result = methodType.Invoke(instance, args.ToArray());
                var resultJson = JsonConvert.SerializeObject(result);
                return Task.FromResult(new Response { Code = "200", Data = resultJson });
            }
            catch (System.Exception ex)
            {
                return Task.FromResult(new Response { Code = "500", Data = "", Message = ex.ToString() });
            }

        }
    }
}