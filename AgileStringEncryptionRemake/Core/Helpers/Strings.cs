using System;
using System.Text;

namespace AgileStringEncryptionRemake.Core.Helpers
{
    public static class Strings
    {
        public static string ProtectString(string source)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
                stringBuilder.Append(Convert.ToChar((int)(source[i] ^ (char)Protections.Runtime.Strings.byteArrayYouKnow[i % Protections.Runtime.Strings.byteArrayYouKnow.Length])));

            return stringBuilder.ToString();
        }
    }
}