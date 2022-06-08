using System;
using System.Collections;
using System.Text;

namespace AgileStringEncryptionRemake.Core.Protections.Runtime
{
    public static class Strings
    {
        internal static string decryptString(string str)
        {
            string result;
            lock (sc)
            {
                if (sc.ContainsKey(str))
                    result = (string)sc[str];
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < str.Length; i++)
                    {
                        stringBuilder.Append(Convert.ToChar((int)(str[i] ^ (char)byteArrayYouKnow[i % byteArrayYouKnow.Length])));
                    }

                    sc[str] = stringBuilder.ToString();
                    result = stringBuilder.ToString();
                }
            }
            return result;
        }

        internal static byte[] byteArrayYouKnow = new byte[] { };

        private static Hashtable sc = new Hashtable();
    }
}