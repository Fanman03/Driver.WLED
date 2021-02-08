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
    public class GenericLEDStrip : WLEDCustomDeviceSpec
    {
        public GenericLEDStrip() : this(300) { }

        public GenericLEDStrip(int leds = 300)
        {
            MadeByName = "Generic";
            LedCount = leds;
            Name = "RGB LED Strip";
            PngData = ImageHelper.ReadImageStream("LedStrip.png");
        }
    }
    public class GenericLedBulbs : WLEDCustomDeviceSpec
    {
        public GenericLedBulbs() : this(100) { }

        public GenericLedBulbs(int leds = 100)
        {
            MadeByName = "Generic";
            LedCount = leds;
            Name = "RGB LED Pixels";
            PngData = ImageHelper.ReadImageStream("LedPixel.png");
        }
    }

    public class GenericLedMatrix : WLEDCustomDeviceSpec
    {
        public GenericLedMatrix() : this(64) { }

        public GenericLedMatrix(int leds = 64)
        {
            MadeByName = "Generic";
            LedCount = leds;
            Name = "RGB LED Matrix";
            PngData = ImageHelper.ReadImageStream("LedMatrix.png");
            GridWidth = 8;
            GridHeight = 8;
        }
    }
}
