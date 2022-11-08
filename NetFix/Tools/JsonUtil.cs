using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace NetFix.Tools
{
    internal static class JsonUtil
    {
        private static readonly Encoding Enc = Encoding.UTF8;

        private static readonly JsonSerializerSettings Config = new()
        {
            Formatting = Formatting.Indented
        };

        public static void Write(string path, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Config);
            File.WriteAllText(path, json, Enc);
        }

        public static T Read<T>(string path)
        {
            var txt = File.ReadAllText(path, Enc);
            return JsonConvert.DeserializeObject<T>(txt, Config);
        }
    }
}