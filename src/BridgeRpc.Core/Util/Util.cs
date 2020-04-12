using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core.Util
{
    public class Util
    {
        public static readonly Random Random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
        
        public static JToken ToJToken<T>(T obj)
        {
            if (obj == null)
                return JValue.CreateNull();
            if (obj is JToken token)
                return token;
            if (obj is string str)
                return str;
            return JToken.FromObject(obj);
        }
    }
}