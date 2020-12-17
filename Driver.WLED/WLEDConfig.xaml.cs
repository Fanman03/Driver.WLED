using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Driver.WLED
{
    /// <summary>
    /// Interaction logic for WLEDConfig.xaml
    /// </summary>
    public partial class WLEDConfig : UserControl
    {
        public WLEDDriver WledDriver;
        public WLEDConfig(WLEDDriver driver)
        {
            WledDriver = driver;
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            WledDriver.AddController(WledDriver.configModel.NewController(IPBox.Text));
        }
    }
}
