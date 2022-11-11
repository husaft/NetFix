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

            var hasTerms = options.Terms.Any();
            foreach (var file in files)
            {
                var label = IoUtil.GetRelative(input, file);
                var ctx = ModuleDef.CreateModuleContext();
                using var mod = TryLoad(file, ctx);
                if (mod == null)
                    continue;

                var asm = mod.Assembly;
                var isDirty = false;
                var refs = mod.GetAssemblyRefs().OrderBy(a => a.FullName).ToArray();
                var isFirst = true;
                foreach (var assRef in refs)
                {
                    var include = !hasTerms || options.Terms.Any(t => t == assRef.Name);
                    if (!include)
                        continue;
                    if (isFirst)
                    {
                        Console.WriteLine($" * {label}");
                        Console.WriteLine($"   - {asm}");
                        isFirst = false;
                    }
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

        private static ModuleDef TryLoad(string file, ModuleContext ctx)
        {
            try
            {
                return ModuleDefMD.Load(file, ctx);
            }
            catch (BadImageFormatException)
            {
                return null;
            }
        }
    }
}