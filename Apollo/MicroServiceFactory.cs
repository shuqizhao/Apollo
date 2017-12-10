using System;
using System.Reflection;
namespace Apollo
{
    public class MicroServiceFactory<T>
    {

        private MicroServiceFactory() { }

        private static T _instance;
        private static readonly object syslock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syslock)
                    {
                        if (_instance == null)
                        {
                            _instance = DispatchProxy.Create<T, InvokeProxy<T>>();
                            return _instance;
                        }
                        else
                        {
                            return _instance;
                        }
                    }
                }
                else
                {
                    return _instance;
                }
            }

        }
    }
}