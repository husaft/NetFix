using System;
using CommandLine;
using CommandLine.Text;
using NetFix.Core;
using NetFix.Tools;

namespace NetFix
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using var parser = new Parser(o =>
            {
                o.AutoHelp = true;
                o.AutoVersion = true;
            });
            var res = parser.ParseArguments<Options>(args);
            var opts = res?.Value;
            if (opts != null)
            {
                if (opts.Patch)
                {
                    Patcher.Start(opts);
                    return;
                }
                if (opts.Inspect)
                {
                    Inspector.Start(opts);
                    return;
                }
            }
            ShowHelp(CmdUtil.ToNotParsed(res));
        }

        private static void ShowHelp(ParserResult<Options> res)
        {
            var help = HelpText.AutoBuild(res);
            Console.WriteLine(help);
        }
    }
}