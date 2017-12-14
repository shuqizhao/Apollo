using System;
using System.Reflection;
using Grpc.Core;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Consul;
using Newtonsoft.Json;

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

        public static T Call<T>(string name, params object[] parameters) where T : class, new()
        {
            try
            {
                if (!name.Contains("."))
                {
                    throw new Exception("Method name must contains .");
                }
                var index = name.LastIndexOf('.');
                var serviceKey = name.Remove(index);
                var methodKey = name;

                var response = Call(serviceKey, methodKey, parameters);

                if (typeof(T) == typeof(void))
                {
                    return default(T);
                }
                var result = JsonHelper.DeserializeJsonToObject<T>(response.Data);
                return result;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            return default(T);
        }

        public static void Call(string name, params object[] parameters)
        {
            try
            {
                if (!name.Contains("."))
                {
                    throw new Exception("Method name must contains .");
                }
                var index = name.LastIndexOf('.');
                var serviceKey = name.Remove(index);
                var methodKey = name;
                Call(serviceKey, methodKey, parameters);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        internal static Response Call(string serviceKey, string methodKey, params object[] parameters)
        {
            var service = ConsulHelper.GetServer(serviceKey);
            var channel = new Channel(service.Address + ":" + service.Port, ChannelCredentials.Insecure);
            //System.Console.WriteLine(service.Address + ":" + service.Port);
            var client = new ApolloService.ApolloServiceClient(channel);
            var request = new Request();
            request.ServiceName = methodKey;
            var jsonInput = "";
            foreach (var arg in parameters)
            {
                jsonInput += JsonConvert.SerializeObject(arg) + "兲";
            }
            request.Data = jsonInput;
            var response = client.Call(request);
            channel.ShutdownAsync().Wait();
            return response;
        }

        private static Dictionary<string, Type> ServiceTypes = new Dictionary<string, Type>();

        private static Dictionary<string, Type> ServiceTypesOfMethodKey = new Dictionary<string, Type>();

        private static Dictionary<string, MethodInfo> MethodTypes = new Dictionary<string, MethodInfo>();
        internal static string BuildServiceKey(Type serviceType)
        {
            var serviceName = "";
            var attributes = serviceType.GetCustomAttributes();
            if (attributes.Any())
            {
                var microServiceAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(MicroServiceAttribute));
                if (microServiceAttribute != null)
                {
                    var microServiceAttr = (MicroServiceAttribute)microServiceAttribute;
                    if (string.IsNullOrWhiteSpace(microServiceAttr.Name))
                    {
                        serviceName = serviceType.FullName;
                    }
                    else
                    {
                        serviceName = microServiceAttr.Name;
                    }
                }
            }
            return serviceName;
        }
        internal static string BuildMethodKey(string serviceName, MethodInfo method)
        {
            var key = "";
            var methodName = "";
            var methodAttriBase = method.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(MicroServiceMethodAttribute));
            if (methodAttriBase != null)
            {
                var methodAttri = (MicroServiceMethodAttribute)methodAttriBase;
                if (!string.IsNullOrWhiteSpace(methodAttri.Name))
                {
                    methodName = methodAttri.Name;
                }
            }
            if (methodName == "")
            {
                key = serviceName + "_" + method.Name;
                foreach (var parameter in method.GetParameters())
                {
                    key += "-" + parameter.ParameterType.FullName;
                }
            }
            else
            {
                key = serviceName + "_" + methodName;
            }
            return key;
        }
        internal static void AddMethodTypes(string serviceName, Type methodType, Type serviceType)
        {
            var methods = methodType.GetMethods();
            foreach (var method in methods)
            {
                var key = BuildMethodKey(serviceName, method);
                if (MethodTypes.Keys.Contains(key))
                {
                    throw new Exception($"Method {key} exists");
                }
                MethodTypes.Add(key, method);
                ServiceTypesOfMethodKey.Add(key, serviceType);
            }
        }

        internal static MethodInfo GetMethodType(string key)
        {
            if (!MethodTypes.Keys.Contains(key))
            {
                throw new Exception("Can't found method " + key);
            }
            return MethodTypes[key];
        }

        internal static bool IsExistsMethodType(string key)
        {
            return MethodTypes.ContainsKey(key);
        }

        internal static bool IsExistsServiceType(string key)
        {
            return ServiceTypes.ContainsKey(key);
        }

        internal static void AddServiceType(string key, Type type)
        {
            ServiceTypes.Add(key, type);
        }

        internal static Type GetServiceType(string key)
        {
            if (!ServiceTypes.Keys.Contains(key))
            {
                throw new Exception("Can't found service " + key);
            }
            return ServiceTypes[key];
        }

        internal static Type GetServiceTypeByMethodKey(string key)
        {
            return ServiceTypesOfMethodKey[key];
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
                    var attributes = ifItem.GetCustomAttributes();
                    if (attributes.Any())
                    {
                        var microServiceAttribute = attributes.First(x => x.GetType() == typeof(MicroServiceAttribute));
                        if (microServiceAttribute != null)
                        {
                            var microServiceAttr = (MicroServiceAttribute)microServiceAttribute;
                            var serviceName = "";
                            if (string.IsNullOrWhiteSpace(microServiceAttr.Name))
                            {
                                serviceName = type.FullName;
                            }
                            else
                            {
                                serviceName = microServiceAttr.Name;
                            }
                            BuildService(serviceName, microServiceAttr);
                            AddServiceType(serviceName, type);
                            AddMethodTypes(serviceName, ifItem, type);
                        }
                    }

                }
            }
        }

        internal static string GetIpAddress()
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
            catch (Exception)
            {
                return "";
            }
        }

        internal static int GetPort(string name = "")
        {
            var port = 0;
            var end = false;
            do
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Random ran = new Random();
                        port = ran.Next(2000, 4000);
                    }
                    else
                    {
                        var md5Str = Md5Helper.StrToMD5(name);
                        var a = (short)(md5Str[0]);
                        var b = (short)(md5Str[md5Str.Length - 1]);
                        port = int.Parse(a + "" + b);
                        name = "";
                    }


                    string host = "127.0.0.1";
                    IPAddress ip = IPAddress.Parse(host);
                    IPEndPoint ipe = new IPEndPoint(ip, port);

                    Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sSocket.Bind(ipe);
                    sSocket.Listen(0);
                    sSocket.Close();
                    end = false;
                }
                catch (Exception)
                {
                    end = true;
                }
            } while (end);



            return port;
        }

        private static void BuildService(string name, MicroServiceAttribute microServiceAttribute)
        {
            var port = 0;
            if (microServiceAttribute.Port != 0)
            {
                port = microServiceAttribute.Port;
            }
            else
            {
                port = GetPort(name);
            }
            var ip = GetIpAddress();
            Server server = new Server
            {
                Services = { ApolloService.BindService(new ApolloServiceImpl()) },
                Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
            };
            server.Start();

            var consulRegist = new AgentServiceRegistration
            {
                ID = name + Guid.NewGuid(),
                Name = name,
                Port = port,
                Address = ip,
                Tags = new string[] { Environment.MachineName },
                Check = new AgentServiceCheck
                {
                    TCP = GetIpAddress() + ":" + ConsulHelper.HealthPort,
                    Interval = new TimeSpan(0, 0, 5),
                    Timeout = new TimeSpan(0, 0, 5)
                }
            };
            ConsulHelper.Registor(consulRegist);
        }
    }
}