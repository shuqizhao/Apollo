using System;
using System.Net;
using System.Net.Sockets;
using Consul;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace Apollo
{
    public class ConsulHelper
    {

        private static List<AgentServiceRegistration> AgentServices = new List<AgentServiceRegistration>();
        public static readonly int HealthPort;

        static ConsulHelper()
        {
            HealthPort = MicroServiceManage.GetPort();
            Task.Run(() =>
            {
                string host = MicroServiceManage.GetIpAddress();
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, HealthPort);

                Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sSocket.Bind(ipe);
                sSocket.Listen(0);

                while (true)
                {
                    try
                    {
                        var send = sSocket.Accept();
                        var sendIpoint = send.RemoteEndPoint.ToString();
                        //Console.WriteLine($"{sendIpoint}Connection");
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex);
                    }
                }
            });
            ConsulClientInstance = new ConsulClient();

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var agentService in AgentServices)
                        {
                            var checks = ConsulClientInstance.Health.Checks(agentService.Name);
                            checks.Wait();
                            var dicChecks = checks.GetAwaiter().GetResult().Response;
                            var unHealthChecks = dicChecks.Where(x => x.Status.Status != "passing").ToList();
                            foreach (var unHealthCheck in unHealthChecks)
                            {
                                ConsulClientInstance.Catalog.Deregister(new CatalogDeregistration
                                {
                                    ServiceID = unHealthCheck.ServiceID,
                                    Node = unHealthCheck.Node
                                });
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex);
                    }
                    Thread.Sleep(5000);
                }
            });
        }

        public static readonly ConsulClient ConsulClientInstance;
        public static void Registor(AgentServiceRegistration agentService)
        {
            AgentServices.Add(agentService);
            ConsulClientInstance.Agent.ServiceRegister(agentService);
        }

        public static AgentService GetServer(string name)
        {
            var services = ConsulClientInstance.Health.Service(name);
            services.Wait();
            var serviceList = services.GetAwaiter().GetResult().Response;

            var checks = ConsulClientInstance.Health.Checks(name);
            checks.Wait();
            var dicChecks = checks.GetAwaiter().GetResult().Response;

            var healthChecks = dicChecks.Where(x => x.Status.Status == "passing").ToList();

            if (healthChecks.Any())
            {
                var length = healthChecks.Count;
                var ran = new Random();
                var index = ran.Next(0, length);
                var check = healthChecks[index];
                var serviceItem = serviceList.First(x => x.Service.ID == check.ServiceID);
                return serviceItem.Service;
            }
            else
            {
                throw new Exception("can't found service " + name);
            }
        }
    }
}