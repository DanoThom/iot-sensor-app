using System.Threading.Tasks;
using BuildAzure.IoT.Adafruit.BME280;

namespace IoT.Sensor
{
    public class Bme280Controller
    {
        private BME280Sensor _bme280;

        public async Task<Bme280Controller> Initialise()
        {
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
