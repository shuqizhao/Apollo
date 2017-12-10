using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Apollo{
    class ApolloServiceImpl : ApolloService.ApolloServiceBase
    {
        public override Task<Response> Call(Request request, ServerCallContext context)
        {
            System.Console.WriteLine("I get message :"+request.ServiceName);
            return Task.FromResult(new Response { Message = "Sevice say Hello " + request.ServiceName });
        }
    }
}