using System;
using Grpc.Core;

namespace Apollo
{
    public class MicroServiceManage
    {
        private MicroServiceManage() { }
        
        static MicroServiceManage()
        {
            Server server = new Server
            {
                Services = { ApolloService.BindService(new ApolloServiceImpl()) },
                Ports = { new ServerPort("localhost", 1101, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public static void Run()
        {

        }
    }
}