using System;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using NetFix.Tools;

namespace NetFix.Core
{
    internal static class Inspector
    {
        internal static void Start(Options options)
        {
            var input = IoUtil.CreateDir(options.Inputs);
            Console.WriteLine($"In  => {input}");
            var output = IoUtil.CreateDir(options.Outputs);
            Console.WriteLine($"Out => {output}");
            Console.WriteLine();

            const SearchOption o = SearchOption.AllDirectories;
            var files = Directory.GetFiles(input, "*.dll", o)
                .Concat(Directory.GetFiles(input, "*.exe", o));

            foreach (var file in files)
            {
                var label = IoUtil.GetRelative(input, file);
                Console.WriteLine($" * {label}");

                var ctx = ModuleDef.CreateModuleContext();
                using var mod = ModuleDefMD.Load(file, ctx);
                var asm = mod.Assembly;
                Console.WriteLine($"   - {asm}");

                var isDirty = false;
                foreach (var assRef in mod.GetAssemblyRefs())
                {
                    Console.WriteLine($"     > {assRef}");
                }

                if (!isDirty)
                    continue;

                var oneName = Path.GetFileName(file);
                var tfPath = Path.Combine(output, oneName);
                var pfPath = tfPath.Replace(".dll", ".pdb")
                    .Replace(".exe", ".pdb");
                var modOpt = new ModuleWriterOptions(mod)
                {
                    WritePdb = true, PdbFileName = pfPath
                };
                mod.Write(tfPath, modOpt);
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
        }
    }
}