using System;
using System.Collections;
using System.Text;

namespace AgileStringEncryptionRemake.Core.Protections.Runtime
{
    public static class Strings
    {
        internal static string decryptString(string A_0, string A_1)
        {
            string result;
            lock (sc)
            {
                if (sc.ContainsKey(A_0))
                {
                    result = (string)sc[A_0];
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < A_0.Length; i++)
                    {
                        stringBuilder.Append(Convert.ToChar((int)(A_0[i] ^ (char)byteArrayYouKnow[i % byteArrayYouKnow.Length])));
                    }

                    sc[A_0] = stringBuilder.ToString();
                    result = stringBuilder.ToString();
                }
            }
            return result;
        }

        internal static byte[] byteArrayYouKnow = new byte[]
        {
        };

        private static Hashtable sc = new Hashtable();
    }
}