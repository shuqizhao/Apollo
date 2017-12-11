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

        public static readonly int HealthPort = 5050;

        static ConsulHelper()
        {
            Task.Run(() =>
            {
                string host = "127.0.0.1";
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, HealthPort);

                Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sSocket.Bind(ipe);
                sSocket.Listen(100);

                while (true)
                {
                    //等待连接并且创建一个负责通讯的socket
                    var send = sSocket.Accept();
                    //获取链接的IP地址
                    var sendIpoint = send.RemoteEndPoint.ToString();
                    Console.WriteLine($"{sendIpoint}Connection");
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            //获取发送过来的消息容器
                            byte[] buffer = new byte[1024 * 1024 * 2];
                            var effective = send.Receive(buffer);
                            //有效字节为0则跳过
                            if (effective == 0)
                            {
                                break;
                            }
                            var str = Encoding.UTF8.GetString(buffer, 0, effective);
                            Console.WriteLine(str);
                            var buffers = Encoding.UTF8.GetBytes("Server Return Message");
                            send.Send(buffers);
                        }
                    });
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
            var services = ConsulClientInstance.Agent.Services();

            services.Wait();

            var dic = services.GetAwaiter().GetResult().Response;
            if (dic.ContainsKey(id))
            {
                return dic[id];
            }
            else
            {
                throw new Exception("can't found service " + id);
            }
        }
    }
}