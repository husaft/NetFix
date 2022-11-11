using CommandLine;

namespace NetFix.Core
{
    // ReSharper disable ClassNeverInstantiated.Global
    public class Options
    {
        [Option('i', "input",
            Required = false, HelpText = "Set input folder.")]
        public string Inputs { get; set; } = nameof(Inputs);

        [Option('o', "output",
            Required = false, HelpText = "Set output folder.")]
        public string Outputs { get; set; } = nameof(Outputs);

        [Option('p', "patch",
            Required = false, HelpText = "Patch according to the given JSON fixes.")]
        public bool Patch { get; set; }

        [Option('l', "inspect",
            Required = false, HelpText = "Inspect all found assemblies for details.")]
        public bool Inspect { get; set; }
    }
}