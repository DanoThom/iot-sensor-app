using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using IoT.Sensor;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestDevice_Pi3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Bme280Controller _bmeController;

        public MainPage()
        {
            this.InitializeComponent();

            _bmeController = new Bme280Controller();

            Task.Run(async () => await _bmeController.Initialise())
                .ContinueWith(async _ =>
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { btnRun.IsEnabled = true; });
                    }, TaskContinuationOptions.NotOnFaulted);
        }

        private async void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            var temp = await _bmeController.GetTemperature();
            txtFeedback.Text = $"Current Temperate is: {temp}°C";
        }
    }
}
