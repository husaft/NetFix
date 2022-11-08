using System;
using NetFix.Tools;
using CommandLine;

namespace NetFix
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using var parser = Parser.Default;
            parser
                .ParseArguments<Options>(args)
                .WithParsed(Start);
        }

        private static void Start(Options options)
        {





            throw new NotImplementedException();
        }
    }
}