using System.Linq;
using System.Reflection;
using CommandLine;

namespace NetFix.Tools
{
    internal static class CmdUtil
    {
        public static NotParsed<T> ToNotParsed<T>(ParserResult<T> res)
        {
            const BindingFlags inp = BindingFlags.Instance | BindingFlags.NonPublic;
            var cStr = typeof(NotParsed<T>).GetConstructors(inp).First();
            var info = res.TypeInfo;
            var err = res.Errors;
            var not = (NotParsed<T>)cStr.Invoke(new object[] { info, err });
            return not;
        }
    }
}