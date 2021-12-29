namespace Pizza.Common
{
    public class EmulationConstants
    {
        //TODO make ints
        public static readonly string ONLY_PROCESS_INJECTION = "ONLY_PROCESS_INJECTION";
        public static readonly string ONLY_VIGEM = "ONLY_VIGEM";
        public static readonly string VIGEM_AND_PROCESS_INJECTION = "VIGEM_AND_PROCESS_INJECTION";

        public static bool IsValidValue(string value)
        {
            return ONLY_VIGEM.Equals(value) ||
                ONLY_PROCESS_INJECTION.Equals(value) ||
                VIGEM_AND_PROCESS_INJECTION.Equals(value);
        }
    }
}
