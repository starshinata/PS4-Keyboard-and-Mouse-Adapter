using Serilog;
using System;
using System.Text;

namespace Pizza.Common
{
    public class ExceptionLogger
    {
        private static readonly string BASE_EXCEPTION = "base exception";
        private static readonly string INNER_EXCEPTION = "inner exception";

        public static void LogException(string message, Exception e)
        {
            LogException(message, e, 0);

            if (e.GetBaseException() != null &&
                e.GetBaseException().Message != null &&
                !e.GetBaseException().Message.Equals(e.Message))
            {
                Log.Error(e.GetBaseException().Message);
                LogException(BASE_EXCEPTION, e.GetBaseException(), 1);
            }

            if (e.InnerException != null &&
                e.InnerException.Message != null &&
                !e.InnerException.Message.Equals(e.Message))
            {
                Log.Error(e.InnerException.Message);
                LogException(INNER_EXCEPTION, e.InnerException, 1);
            }

        }

        private static void LogException(string message, Exception e, int counter)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("ERROR!");
            sb.Append(Environment.NewLine);

            sb.Append(String.Format("{0}: {1}{2}", "catcher message", message, Environment.NewLine));
            sb.Append(String.Format("{0}: {1}{2}", "error message", e.Message, Environment.NewLine));
            sb.Append(String.Format("{0}: {1}{2}", "error classType", e.GetType().ToString(), Environment.NewLine));
            sb.Append(String.Format("{0}: {1}{2}", "error data", DictionaryUtil.ToString(e.Data), Environment.NewLine));
            sb.Append(String.Format("{0}: {1}{2}", "error hResult", e.HResult, Environment.NewLine));
            sb.Append(String.Format("{0}: {1}{2}", "error nesting level", counter, Environment.NewLine));
            sb.Append(String.Format("{0}: {1}{2}", "error sourceNamespace", e.Source, Environment.NewLine));

            sb.Append("error stackTrace:");
            sb.Append(Environment.NewLine);
            sb.Append(e.StackTrace);

            Log.Error(sb.ToString());

            if (e.GetBaseException() != null &&
                e.GetBaseException().Message != null &&
                !e.GetBaseException().Message.Equals(e.Message))
            {
                Log.Error(e.GetBaseException().Message);
                LogException(BASE_EXCEPTION, e.GetBaseException(), counter + 1);
            }

            if (e.InnerException != null &&
                e.InnerException.Message != null &&
                !e.InnerException.Message.Equals(e.Message))
            {
                Log.Error(e.InnerException.Message);
                LogException(INNER_EXCEPTION, e.InnerException, counter + 1);
            }
        }

    }
}
