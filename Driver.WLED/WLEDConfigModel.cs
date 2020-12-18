using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleLed;

namespace Driver.WLED
{
    public class WLEDConfigModel : SLSConfigData
    {
        public WLEDController NewController(string IP)
        {
            WLEDController newController = new WLEDController() {IP = IP, Port = "21324"};
            Controllers.Add(newController);
            OnPropertyChanged("Controllers");
            DataIsDirty = true;
            return newController;
        }

        private ObservableCollection<WLEDController> controllers = new ObservableCollection<WLEDController>(){};

        public ObservableCollection<WLEDController> Controllers
        {
            get => controllers;
            set
            {
                SetProperty(ref controllers, value);
                DataIsDirty = true;
            }
        }

        public class WLEDController
        {
            public string IP { get; set; }

            public string Port
            {
                get
                {
                    return Info.udpport.ToString();
                }
                set {}
            }

            private WLEDApiInfo Info
            {
                get
                {
                    try
                    {
                        string jsonString;
                        using (var wc = new QuickClient())
                            jsonString = wc.DownloadString("http://" + IP + "/json/info");
                        var response = JsonConvert.DeserializeObject<WLEDApiInfo>(jsonString);
                        return response;
                    }
                    catch
                    {
                        WLEDApiInfo response = new WLEDApiInfo();
                        response.leds.count = 10;
                        response.name = "Unnamed WLED";
                        response.arch = "Unknown";
                        return response;
                    }
                }
            }
            public int LedCount
            {
                get
                {
                 return Info.leds.count;
                }
                private set
                {
                }
            }
            public string Name
            {
                get
                {
                    return Info.name;
                }
                private set
                {
                }
            }
            public string ControllerType
            {
                get
                {
                    return Info.arch;
                }
                private set
                {
                }
            }
        }

        public class WLEDApiInfo
        {
            public string ver { get; set; }
            public int vid { get; set; }
            public Leds leds { get; set; }
            public bool str { get; set; }
            public string name { get; set; }
            public int udpport { get; set; }
            public bool live { get; set; }
            public string lm { get; set; }
            public string lip { get; set; }
            public int ws { get; set; }
            public int fxcount { get; set; }
            public int palcount { get; set; }
            public Wifi wifi { get; set; }
            public Fs fs { get; set; }
            public string arch { get; set; }
            public string core { get; set; }
            public int lwip { get; set; }
            public int freeheap { get; set; }
            public int uptime { get; set; }
            public int opt { get; set; }
            public string brand { get; set; }
            public string product { get; set; }
            public string mac { get; set; }
        }

        public class Leds
        {
            public int count { get; set; }
            public bool rgbw { get; set; }
            public bool wv { get; set; }
            public List<int> pin { get; set; }
            public int pwr { get; set; }
            public int maxpwr { get; set; }
            public int maxseg { get; set; }
            public bool seglock { get; set; }
        }

        public class Wifi
        {
            public string bssid { get; set; }
            public int rssi { get; set; }
            public int signal { get; set; }
            public int channel { get; set; }
        }

        public class Fs
        {
            public int u { get; set; }
            public int t { get; set; }
            public int pmt { get; set; }
        }

        private class QuickClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = (int) TimeSpan.FromSeconds(2).TotalMilliseconds;
                return w;
            }
        }
    }
}
