using System;
using System.Net;
using System.Net.Sockets;
using Consul;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Apollo
{
    public class ConsulHelper
    {

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
                    //等待连接并且创建一个负责通讯的socket
                    var send = sSocket.Accept();
                    //获取链接的IP地址
                    var sendIpoint = send.RemoteEndPoint.ToString();
                    Console.WriteLine($"{sendIpoint}Connection");
                }
            });
            ConsulClientInstance = new ConsulClient();
        }

        public static readonly ConsulClient ConsulClientInstance;
        public static void Registor(AgentServiceRegistration agentService)
        {
            ConsulClientInstance.Agent.ServiceRegister(agentService);
        }

        public static AgentService GetServer(string id)
        {
            var services = ConsulClientInstance.Health.Service(id);
            services.Wait();
            var serviceList = services.GetAwaiter().GetResult().Response;


            var checks = ConsulClientInstance.Health.Checks(id);
            checks.Wait();
            var dicChecks = checks.GetAwaiter().GetResult().Response;

            var healthChecks = dicChecks.Where(x => x.Status.Status == "passing").ToList();

            var unHealthChecks = dicChecks.Where(x => x.Status.Status != "passing").ToList();
            foreach (var unHealthCheck in unHealthChecks)
            {
                ConsulClientInstance.Catalog.Deregister(new CatalogDeregistration
                {
                    ServiceID = unHealthCheck.ServiceID,
                    Node = unHealthCheck.Node
                });
            }

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
                throw new Exception("can't found service " + id);
            }
        }
    }
}