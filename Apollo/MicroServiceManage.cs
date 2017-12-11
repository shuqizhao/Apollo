using System;
using System.Reflection;
using Grpc.Core;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Consul;

namespace Apollo
{
    public class MicroServiceManage
    {
        private MicroServiceManage() { }

        static MicroServiceManage()
        {

        }
        private static string _address = "";
        public static void Run(string address = "")
        {
            _address = address;
            SelfConstruction();
        }

        private static Dictionary<string, Type> ServiceTypes = new Dictionary<string, Type>();

        public static void AddServiceType(string key, Type type)
        {
            ServiceTypes.Add(key, type);
        }

        public static Type GetServiceType(string key)
        {
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
                        BuildService(ifItem.FullName, (MicroServiceAttribute)microServiceAttribute);
                        AddServiceType(ifItem.FullName, type);
                    }
                }
            }
        }

        private static string GetIpAddress()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static int GetPort()
        {
            var port = 0;
            var end = false;
            do
            {
                try
                {
                    Random ran = new Random();
                    port = ran.Next(2000, 4000);

                    string host = "127.0.0.1";
                    IPAddress ip = IPAddress.Parse(host);
                    IPEndPoint ipe = new IPEndPoint(ip, port);

                    Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sSocket.Bind(ipe);
                    sSocket.Listen(0);
                    sSocket.Close();
                    end = false;
                }
                catch (Exception ex)
                {
                    end = true;
                }
            } while (end);



            return port;
        }

        private static void BuildService(string name, MicroServiceAttribute microServiceAttribute)
        {
            var port = GetPort();
            var ip = GetIpAddress();
            Server server = new Server
            {
                Services = { ApolloService.BindService(new ApolloServiceImpl()) },
                Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
            };
            server.Start();

            var consulRegist = new AgentServiceRegistration
            {
                ID = name,
                Name = name + "_" + Dns.GetHostName(),
                Port = port,
                Address = ip,
                Check = new AgentServiceCheck
                {
                    TCP = GetIpAddress() + ":" + ConsulHelper.HealthPort,
                    Interval=new TimeSpan(0,0,5),
                    Timeout=new TimeSpan(0,0,5)
                }
            };
            ConsulHelper.Registor(consulRegist);
        }
    }
}