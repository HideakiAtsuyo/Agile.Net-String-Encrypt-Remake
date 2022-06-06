using System;
using System.Collections.Generic;

namespace AgileStringEncryptionRemake
{
    internal class Utils
    {
        internal static IEnumerable<byte> RandomBytesArray()
        {
            var random = new Random();
            byte[] buffer = new byte[32];
            while (true)
            {
                random.NextBytes(buffer);
                foreach (var ret in buffer)
                    yield return ret;
            }
        }
    }
}