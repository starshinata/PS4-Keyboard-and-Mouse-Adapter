using CommandLine;


namespace RemotePlayInjector
{
    public class Options
    {
        [Option('e', "emulationMode", Required = true, HelpText = "what emulation mode")]
        public int EmulationMode { get; set; }

        [Option('p', "processId", Required = true, HelpText = "Process Id of the instance of RemotePlay we want to inject into")]
        public int ProcessId { get; set; }

    }
}
