namespace Pizza.Common
{
    public class EmulationConstants
    {
        public static readonly int ONLY_PROCESS_INJECTION = 1;
        public static readonly int ONLY_VIGEM = 2;
        public static readonly int VIGEM_AND_PROCESS_INJECTION = 3;

        public static bool IsValidValue(int value)
        {
            return ONLY_VIGEM == value ||
                ONLY_PROCESS_INJECTION == value ||
                VIGEM_AND_PROCESS_INJECTION == value;
        }

        public static string ToString(int value)
        {
            if (ONLY_VIGEM == value)
            {
                return "ONLY_VIGEM";
            }
            if (ONLY_PROCESS_INJECTION == value)
            {
                return "ONLY_PROCESS_INJECTION";
            }
            if (VIGEM_AND_PROCESS_INJECTION == value)
            {
                return "VIGEM_AND_PROCESS_INJECTION";
            }

            return "UNKNOWN";
        }
    }
}
