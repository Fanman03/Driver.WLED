using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace Driver.WLED.CustomDeviceSpecs
{
    public class WLEDCustomDeviceSpec : CustomDeviceSpecification
    {
        public WLEDCustomDeviceSpec()
        {
            this.RGBOrder = RGBOrder.RGB;
            this.MapperName = null;
        }
    }
    public class Generic300LedStrip : WLEDCustomDeviceSpec
    {
        public Generic300LedStrip() : this(300) { }

        public Generic300LedStrip(int leds = 300)
        {
            MadeByName = "Generic";
            LedCount = leds;
            Name = "300 LED RGB Strip";
            //PngData = ImageHelper.ReadImageStream("SPFan.png");
        }
    }
}
