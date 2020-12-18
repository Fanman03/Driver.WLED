using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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

        public static Assembly assembly = Assembly.GetExecutingAssembly();
        public static Stream GenericStream = assembly.GetManifestResourceStream("Driver.WLED.WLED.png");
        public static Stream Esp32Stream = assembly.GetManifestResourceStream("Driver.WLED.ESP32.png");
        public static Stream Esp8266Stream = assembly.GetManifestResourceStream("Driver.WLED.ESP8266.png");
        public List<WLEDControlDevice> deviceList = new List<WLEDControlDevice>();



        public void Configure(DriverDetails driverDetails)
        {
            ConvertControllersToDevices();
        }

        public void ConvertControllersToDevices()
        {
            foreach (WLEDConfigModel.WLEDController controller in configModel.Controllers)
            {
                AddController(controller);
            }
        }

        public void AddController(WLEDConfigModel.WLEDController controller)
        {
            WLEDControlDevice wled = new WLEDControlDevice();
            wled.Name = controller.Name;
            wled.DeviceType = DeviceTypes.LedStrip;
            wled.Driver = this;
            wled.Has2DSupport = false;
            wled.LedCount = controller.LedCount;
            wled.ConnectedTo =controller.ControllerType.ToUpper();
            if (wled.ConnectedTo == "ESP32")
            {
                wled.ProductImage = (Bitmap)System.Drawing.Image.FromStream(Esp32Stream);
            } else if (wled.ConnectedTo == "ESP8266")
            {
                wled.ProductImage = (Bitmap)System.Drawing.Image.FromStream(Esp8266Stream);
            }
            else
            {
                wled.ProductImage = (Bitmap)System.Drawing.Image.FromStream(GenericStream);
            }
            wled.Endpoint = new IPEndPoint(IPAddress.Parse(controller.IP), Int32.Parse(controller.Port));

            List<ControlDevice.LedUnit> deviceLeds = new List<ControlDevice.LedUnit>();
            for (int i = 0; i < controller.LedCount; i++)
            {
                ControlDevice.LedUnit newLed = new ControlDevice.LedUnit();
                newLed.Data = new ControlDevice.LEDData();
                newLed.Data.LEDNumber = i;
                deviceLeds.Add(newLed);
            }

            wled.LEDs = deviceLeds.ToArray();
            DeviceAdded?.Invoke(wled, new Events.DeviceChangeEventArgs(wled));
            deviceList.Add(wled);
            var helloWorld = new List<byte>() { 0x02, 0xFF, 0x00, 0xFF, 0x00 };
            wled.Socket.SendTo(helloWorld.ToArray(), wled.Endpoint);
        }

        public void Dispose()
        {
            foreach (WLEDControlDevice device in deviceList)
            {
                var clear_bytes = new List<byte>() { 0x02, 0x01, 0x00, 0x00, 0xFF };
                device.Socket.SendTo(clear_bytes.ToArray(), device.Endpoint);
            }
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

            wledDevice.Socket.SendTo(send_bytes.ToArray(), wledDevice.Endpoint);
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {
            ConvertControllersToDevices();
            this.configModel = config as WLEDConfigModel;
        }

        public UserControl GetCustomConfig(ControlDevice controlDevice)
        {
            var config = new WLEDConfig(this)
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
        }

        public void SetColorProfile(ColorProfile value)
        {

        }

        public class WLEDControlDevice : ControlDevice
        {
            public EndPoint Endpoint { get; set; }
            public Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            public int LedCount { get; set; }
        }
    }
}
