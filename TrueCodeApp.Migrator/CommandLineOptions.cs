using CommandLine;

namespace TrueCodeApp.Migrator;

public class CommandLineOptions
{
    [Option("dryrun", Required = false, HelpText = "Without migration")]
    public bool DryRun { get; set; }

    [Option('c', "connection", Required = false, HelpText = "Connection string")]
    public string? ConnectionString { get; set; }

    [Option('e', "env", Required = false, HelpText = "Environment")]
    public string? Environment { get; set; }
}
