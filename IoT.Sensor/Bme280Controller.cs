using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using BuildAzure.IoT.Adafruit.BME280;

namespace IoT.Sensor
{
    public class Bme280Controller
    {
        private BME280Sensor _bme280;

        public async Task<Bme280Controller> Initialise()
        {
            var gpio = GpioController.GetDefault()
                       ?? throw new NullReferenceException("Unable to find GpioController.");

            GpioPin ledPin = gpio.OpenPin(24);
            
            _bme280 = new BME280Sensor();
            await _bme280.Initialize();
            return this;
        }

        public async Task<double> GetTemperature()
        {
            return await _bme280.ReadTemperature();
        }
    }
}
