using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using SimpleLed;

namespace Driver.WLED
{
    public class WLEDDriver : ISimpleLedWithConfig
    {
        public event Events.DeviceChangeEventHandler DeviceAdded;
        public event Events.DeviceChangeEventHandler DeviceRemoved;

        [JsonIgnore]
        public WLEDConfigModel configModel = new WLEDConfigModel();

        public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public static Assembly assembly = Assembly.GetExecutingAssembly();
        public static Stream imageStream = assembly.GetManifestResourceStream("Driver.WLED.WLED.png");

        public static WLEDControlDevice wled = new WLEDControlDevice();
        
        public void Configure(DriverDetails driverDetails)
        {
            wled.Name = "WLED";
            wled.DeviceType = DeviceTypes.LedStrip;
            wled.Driver = this;
            wled.Has2DSupport = false;
            wled.ProductImage = (Bitmap) System.Drawing.Image.FromStream(imageStream);
            wled.LedCount = configModel.LedCount;
            wled.Endpoint = new IPEndPoint(IPAddress.Parse(configModel.IP), Int32.Parse(configModel.Port));

            List <ControlDevice.LedUnit> deviceLeds = new List<ControlDevice.LedUnit>();
            for (int i = 0; i < configModel.LedCount; i++)
            {
                ControlDevice.LedUnit newLed = new ControlDevice.LedUnit();
                newLed.Data = new ControlDevice.LEDData();
                newLed.Data.LEDNumber = i;
                deviceLeds.Add(newLed);
            }

            wled.LEDs = deviceLeds.ToArray();
            DeviceAdded?.Invoke(wled, new Events.DeviceChangeEventArgs(wled));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T GetConfig<T>() where T : SLSConfigData
        {
            WLEDConfigModel data = this.configModel;
            SLSConfigData proxy = data;
            return (T)proxy;
        }

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                SupportsPull = false,
                SupportsPush = true,
                IsSource = false,
                SupportsCustomConfig = true,
                Id = Guid.Parse("c7204793-c45a-4b8f-8290-56e66e4861a7"),
                Author = "Fanman03",
                Blurb = "Driver for controlling WLED controllers. WLED controller software by Aircoookie.",
                CurrentVersion = new ReleaseNumber(1, 0, 0, 3),
                GitHubLink = "https://github.com/SimpleLed/Driver.WLED",
                IsPublicRelease = false,
            };
        }

        public void InterestedUSBChange(int VID, int PID, bool connected)
        {
            throw new NotImplementedException();
        }

        public string Name()
        {
            return "WLED";
        }

        public void Pull(ControlDevice controlDevice)
        {
            throw new NotImplementedException();
        }

        public void Push(ControlDevice controlDevice)
        {
            WLEDControlDevice wledDevice = (WLEDControlDevice) controlDevice;

            var send_bytes = new List<byte>() { 0x02, 0x1E };

            for (int i = 0; i < wledDevice.LEDs.Length; i++)
            {
                byte r = (byte)wledDevice.LEDs[i].Color.Red;
                send_bytes.Add(r);
                byte g = (byte)wledDevice.LEDs[i].Color.Green;
                send_bytes.Add(g);
                byte b = (byte)wledDevice.LEDs[i].Color.Blue;
                send_bytes.Add(b);
            }

            socket.SendTo(send_bytes.ToArray(), wledDevice.Endpoint);
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {
            this.configModel = config as WLEDConfigModel;
        }

        public UserControl GetCustomConfig(ControlDevice controlDevice)
        {
            var config = new WLEDConfig()
            {
                DataContext = configModel
            };

            return config;
        }

        public bool GetIsDirty()
        {
            return configModel.DataIsDirty;
        }

        public void SetIsDirty(bool val)
        {
            configModel.DataIsDirty = val;

            wled.Endpoint = new IPEndPoint(IPAddress.Parse(configModel.IP), Int32.Parse(configModel.Port));
            List<ControlDevice.LedUnit> deviceLeds = new List<ControlDevice.LedUnit>();
            for (int i = 0; i < configModel.LedCount; i++)
            {
                ControlDevice.LedUnit newLed = new ControlDevice.LedUnit();
                newLed.Data = new ControlDevice.LEDData();
                newLed.Data.LEDNumber = i;
                deviceLeds.Add(newLed);
            }

            wled.LEDs = deviceLeds.ToArray();
        }

        public void SetColorProfile(ColorProfile value)
        {

        }

        public class WLEDControlDevice : ControlDevice
        {
            public EndPoint Endpoint { get; set; }
            public int LedCount { get; set; }
        }
    }
}
