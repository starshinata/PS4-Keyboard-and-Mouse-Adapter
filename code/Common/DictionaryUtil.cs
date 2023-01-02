using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pizza.Common
{
    public class DictionaryUtil
    {
        public static string ToString(IDictionary dictionary)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool first = true;

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                if (first)
                {
                    sb.Append(Environment.NewLine);
                    first = false;
                }
                sb.Append(String.Format("    {0}={1}{2}", pair.Key, pair.Value, Environment.NewLine));
            }

            sb.Append("}");
            return sb.ToString();
        }
    }
}
