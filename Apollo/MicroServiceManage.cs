using System;
using System.Reflection;
using Grpc.Core;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Apollo
{
    public class MicroServiceManage
    {
        private MicroServiceManage() { }

        static MicroServiceManage()
        {
            SelfConstruction();
        }

        public static void Run()
        {

        }

        private static Dictionary<string,Type> ServiceTypes =new Dictionary<string,Type>();

        public static void AddServiceType(string key,Type type){
            ServiceTypes.Add(key,type);
        }

         public static Type GetServiceType(string key){
            return ServiceTypes[key];
        }

        private static void SelfConstruction()
        {
            var ass = Assembly.GetEntryAssembly();
            var types = ass.GetTypes();
            foreach (var type in types)
            {
                var ifs = type.GetInterfaces();
                foreach (var ifItem in ifs)
                {
                    var microServiceAttribute = ifItem.GetCustomAttributes().First(x => x.GetType() == typeof(MicroServiceAttribute));
                    if (microServiceAttribute != null)
                    {
                        BuildService((MicroServiceAttribute)microServiceAttribute);
                        AddServiceType(ifItem.FullName,type);
                    }
                }
            }
        }

        private static void BuildService(MicroServiceAttribute microServiceAttribute)
        {
            Server server = new Server
            {
                Services = { ApolloService.BindService(new ApolloServiceImpl()) },
                Ports = { new ServerPort("localhost", 5001, ServerCredentials.Insecure) }
            };
            server.Start();
        }
    }
}