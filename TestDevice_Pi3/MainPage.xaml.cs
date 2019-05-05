using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
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
using IoT.Hub;
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
        private GpioPin _ledPin;
        private const double TempThreshold = 24.1;
        private const int LedPin = 24;

        public MainPage()
        {
            this.InitializeComponent();

            try
            {
                _bmeController = new Bme280Controller();

                var task = Task.Run(async () =>
                    {
                        await Task.WhenAll(_bmeController.Initialise(), IoTHub.ConnectToHub());
                    })
                    .ContinueWith(
                        async _ =>
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () => { btnRun.IsEnabled = true; });
                        }, TaskContinuationOptions.NotOnFaulted);

                if (task.IsFaulted)
                {
                    txtFeedback.Text =
                        $"An error occurred while initialising the device.\r\n{string.Join(" | ", task.Exception.InnerExceptions.Select(x => x.Message).ToList())}";
                    return;
                }

                var gpio = GpioController.GetDefault()
                           ?? throw new NullReferenceException("Unable to find GpioController on this device.");

                _ledPin = gpio.OpenPin(LedPin);
                _ledPin.Write(GpioPinValue.Low);
                _ledPin.SetDriveMode(GpioPinDriveMode.Output);
            }
            catch (Exception ex)
            {
                txtFeedback.Text = ex.Message;
            }
        }

        private async void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            if (_ledPin.Read() == GpioPinValue.High)
            {
                _ledPin.Write(GpioPinValue.Low);
            }

            var temp = await _bmeController.GetTemperature();

            try
            {
                //await IoTHub.SendDeviceToCloudMessage(temp);
            } catch (Exception ex) {
                txtFeedback.Text = ex.Message;
            }

            if (temp > TempThreshold)
            {
                txtFeedback.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                _ledPin.Write(GpioPinValue.High);
            }
            else
            {
                txtFeedback.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            }

            txtFeedback.Text = $"Temperate is: {Math.Round(temp, 2)}°C";
        }

    }
}
