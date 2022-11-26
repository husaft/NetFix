using System.IO;
using dnlib.DotNet;

namespace NetFix.Tools
{
    internal static class IoUtil
    {
        public static string CreateDir(string path)
        {
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path!);
            return path;
        }

        public static string GetRelative(string dir, string file)
        {
            return file
                .Replace(dir, string.Empty)
                .TrimStart('\\', '/');
        }

        public static string Simple(string txt)
            => txt.Replace("System.Void", "void")
                .Replace("System.String", "string")
                .Replace("System.Boolean", "bool")
                .Replace("System.Int32", "int")
                .Replace("System.Int64", "long")
                .Replace("System.Collections.Generic.", "")
                .Replace("Newtonsoft.Json.Linq.", string.Empty);

        public static string ToStr(IMemberDef method, TypeDef type)
        {
            return Simple(method.ToString()
                .Replace($"{type.FullName}::", string.Empty)
                .Replace($"{type.Namespace}.", string.Empty));
        }
    }
}