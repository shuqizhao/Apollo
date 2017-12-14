using System;
using System.IO;
using Newtonsoft.Json;

namespace Apollo{
    public class JsonHelper{
        
        public static T DeserializeJsonToObject<T>(string json) where T : class,new()
        {
            var serializer = new JsonSerializer();
            var sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        public static object DeserializeJsonToObject(string json,Type type) 
        {
            var serializer = new JsonSerializer();
            var sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), type);
            return o;
        }
    }
}