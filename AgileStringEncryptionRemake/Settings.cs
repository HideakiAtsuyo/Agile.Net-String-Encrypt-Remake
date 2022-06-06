using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileStringEncryptionRemake
{
    internal class Settings
    {
        public static string whatAbadKindOfWatermark = Convert.ToBase64String(Encoding.UTF8.GetBytes("github.com/HideakiAtsuyo"));
    }
}