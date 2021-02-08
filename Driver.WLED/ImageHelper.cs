using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.WLED
{
    class ImageHelper
    {
        internal static byte[] ReadImageStream(string name)
        {
            Stream imgStream = System.Reflection.Assembly.GetAssembly(typeof(WLEDDriver)).GetManifestResourceStream("Driver.WLED.CustomDeviceImages" + name);
            var temp = new byte[imgStream.Length];
            imgStream.Read(temp, 0, (int)imgStream.Length);

            return temp;
        }
    }
}
