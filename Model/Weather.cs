using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CrytonCore.Model
{
    public class Weather
    {
        public WeatherInfo WholeForecast { get; set; }
        public SingleWeather ActualWeather { get; set; }
        private string ActualWeatherIcon;
        private readonly Web web = new();

        private void DownloadWeatherForecast((double lat, double lon) geoLocation)
        {
            var weatherUrlStart = "http://www.7timer.info/bin/api.pl?";
            StringBuilder stringBuilder = new(weatherUrlStart);
            _ = stringBuilder.Append("lon=" + geoLocation.lon + "&lat=" + geoLocation.lat);
            _ = stringBuilder.Append("&product=civil&output=json");
            string info = new WebClient().DownloadString(stringBuilder.ToString());

            SetWholeForecast(JsonConvert.DeserializeObject<WeatherInfo>(info));
            SetCurrentWeather(FindCurrnetWeather());
            SetCurrentWeatherIcon();
        }

        private void SetCurrentWeatherIcon()
        {
            var cloundIndex = int.Parse(ActualWeather.Cloudcover);
            var liftedIndex = int.Parse(ActualWeather.LiftedIndex);

            switch (ActualWeather.PrecType.ToString())
            {
                case "rain":
                    if(liftedIndex < -5)
                        ActualWeatherIcon = "/CrytonCore;component/Assets/HeavyStorm.png";
                    else
                        ActualWeatherIcon = "/CrytonCore;component/Assets/Rain.png";
                    return;
                case "snow":
                    ActualWeatherIcon = "/CrytonCore;component/Assets/Snow.png";
                    return;
                case "none":
                    if (cloundIndex < 3 && liftedIndex > -5) 
                        ActualWeatherIcon = "/CrytonCore;component/Assets/Sun.png";
                    if (cloundIndex > 2 && cloundIndex < 9 && liftedIndex > -5)
                        ActualWeatherIcon = "/CrytonCore;component/Assets/PartialSun.png";
                    if (cloundIndex > 8 && liftedIndex > -5)
                        ActualWeatherIcon = "/CrytonCore;component/Assets/Cloud.png";
                    if (liftedIndex < -5)
                        ActualWeatherIcon = "/CrytonCore;component/Assets/Storm.png";
                    break;
                default:
                    break;
            }
        }

        public SingleWeather GetActualWeather()
        {
            DownloadWeatherForecast(web.GetGlobalCoordinates());
            return ActualWeather;
        }

        public WeatherInfo GetWholeForecast()
        {
            DownloadWeatherForecast(web.GetGlobalCoordinates());
            return WholeForecast;
        }

        public string GetActualWeatherIcon()
        {
            DownloadWeatherForecast(web.GetGlobalCoordinates());
            return ActualWeatherIcon;
        }

        private SingleWeather FindCurrnetWeather()
        {
            var initDate = WholeForecast.Init;
            DateTime firstDate = DateTime.Now;
            DateTime secondDate = new(
                year: int.Parse(initDate.Substring(0, 4)),
                month: int.Parse(initDate.Substring(4, 2)),
                day: int.Parse(initDate.Substring(6, 2)),
                hour: int.Parse(initDate.Substring(8, 2)),
                minute: 00,
                second: 00);

            TimeSpan diff = secondDate.Subtract(firstDate);
            var diffHours = diff.TotalHours;
            var index = Math.Floor(Math.Abs(diffHours) / 3);
            return WholeForecast.Dataseries.Single(x => x.Timepoint ==(index*3).ToString());
        }

        private void SetWholeForecast(WeatherInfo weatherInfo)
        {
            WholeForecast = weatherInfo;
        }
        private void SetCurrentWeather(SingleWeather singleWeather)
        {
            ActualWeather = singleWeather;
        }
        public class WeatherInfo
        {

            [JsonProperty("product")]
            public string Product { get; set; }

            [JsonProperty("init")]
            public string Init { get; set; }

            [JsonProperty("dataseries")]
            public IEnumerable<SingleWeather> Dataseries { get; set; }
        }

        public class SingleWeather
        {

            [JsonProperty("timepoint")]
            public string Timepoint { get; set; }

            [JsonProperty("cloudcover")]
            public string Cloudcover { get; set; }

            [JsonProperty("seeing")]
            public string Seeing { get; set; }

            [JsonProperty("transparency")]
            public string Transparency { get; set; }

            [JsonProperty("lifted_index")]
            public string LiftedIndex { get; set; }

            [JsonProperty("rh2m")]
            public string Rh2m { get; set; }

            [JsonProperty("wind10m")]
            public Wind Wind10m { get; set; }

            [JsonProperty("temp2m")]
            public string Temp { get; set; }

            [JsonProperty("prec_type")]
            public string PrecType { get; set; }
        }

        public class Wind
        {
            [JsonProperty("direction")]
            public string Direction { get; set; }

            [JsonProperty("speed")]
            public string Speed { get; set; }
        }
    }
}
