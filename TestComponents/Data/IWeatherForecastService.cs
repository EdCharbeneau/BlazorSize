using System;
using System.Threading.Tasks;

namespace TestComponents
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);
    }
}