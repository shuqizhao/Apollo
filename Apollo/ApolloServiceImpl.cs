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
                var methodType = MicroServiceManage.GetMethodType(request.ServiceName);
                
                var serverType = MicroServiceManage.GetServiceTypeByMethodKey(request.ServiceName);

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
                var instance = Activator.CreateInstance(serverType);
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