using CommandLine;

namespace NetFix.Tools
{
    public class Options
    {
        [Option('v', "verbose",
            Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('i', "input",
            Required = false, HelpText = "Set input folder")]
        public string Inputs { get; set; } = nameof(Inputs);

        [Option('o', "output",
            Required = false, HelpText = "Set output folder")]
        public string Outputs { get; set; } = nameof(Outputs);
    }
}