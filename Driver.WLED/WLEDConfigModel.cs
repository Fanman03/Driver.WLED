using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace Driver.WLED
{
    public class WLEDConfigModel : SLSConfigData
    {
        private string ip = "0.0.0.0";
        public string IP
        {
            get => ip;
            set
            {
                SetProperty(ref ip, value);
                DataIsDirty = true;
            }
        }

        private string port = "21324";
        public string Port
        {
            get => port;
            set
            {
                SetProperty(ref port, value);
                DataIsDirty = true;
            }
        }

        private int ledCount = 10;

        public int LedCount
        {
            get => ledCount;
            set
            {
                SetProperty(ref ledCount, value);
                DataIsDirty = true;
            }
        }
    }
}
