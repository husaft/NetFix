using System;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using NetFix.Model;
using NetFix.Tools;

namespace NetFix.Core
{
    internal static class Patcher
    {
        internal static void Start(Options options)
        {
            var input = IoUtil.CreateDir(options.Inputs);
            Console.WriteLine($"In  => {input}");
            var output = IoUtil.CreateDir(options.Outputs);
            Console.WriteLine($"Out => {output}");
            Console.WriteLine();

            const SearchOption o = SearchOption.AllDirectories;
            var files = Directory.GetFiles(input, "*.fix.json", o);

            foreach (var file in files)
            {
                var label = IoUtil.GetRelative(input, file);
                Console.WriteLine($" * {label}");

                var patch = JsonUtil.Read<Patch>(file);
                foreach (var oneFile in patch.Files)
                {
                    var ofPath = Path.Combine(input, oneFile.Name);
                    Console.WriteLine($"   - {oneFile.Name}");

                    var ctx = ModuleDef.CreateModuleContext();
                    using var mod = ModuleDefMD.Load(ofPath, ctx);
                    var asm = mod.Assembly;
                    Console.WriteLine($"     - {asm}");

                    var isDirty = false;
                    foreach (var oneType in mod.GetTypes())
                    {
                        var oSpace = oneFile.Spaces.FirstOrDefault(s =>
                            s.Name.Equals(oneType.Namespace));
                        var oType = oSpace?.Types.FirstOrDefault(s =>
                            s.Name.Equals(oneType.Name));
                        if (oType == null)
                            continue;

                        Console.WriteLine($"       - {oneType}");
                        if (oType.Public?.Contains("@") ?? false)
                        {
                            oneType.Attributes |= TypeAttributes.Public;
                            if (oneType.IsSealed)
                                oneType.Attributes &= ~TypeAttributes.Sealed;
                            Console.WriteLine($"         -> {oneType.Attributes}");
                            isDirty = true;
                        }

                        foreach (var oneMeth in oneType.Methods)
                        {
                            var mName = oneMeth.Name;
                            var pro = oType.Protected?.FirstOrDefault(p => p.Equals(mName));
                            var pub = oType.Public?.FirstOrDefault(p => p.Equals(mName));

                            if (pro == null && pub == null)
                                continue;

                            var debug = IoUtil.ToStr(oneMeth, oneType);
                            Console.WriteLine($"         > {debug}");

                            oneMeth.Access = pro != null
                                ? MethodAttributes.Family
                                : MethodAttributes.Public;
                            isDirty = true;
                        }
                    }

                    if (!isDirty)
                        continue;

                    var tfPath = Path.Combine(output, oneFile.Name);
                    var pfPath = tfPath.Replace(".dll", ".pdb");
                    var modOpt = new ModuleWriterOptions(mod)
                    {
                        WritePdb = true, PdbFileName = pfPath
                    };
                    mod.Write(tfPath, modOpt);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
        }
    }
}