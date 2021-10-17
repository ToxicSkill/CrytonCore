using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace CrytonCore.Model
{
    public class Weather
    {
        public WeatherInfo WholeForecast { get; set; }
        public SingleWeather CurrnetWeather { get; set; }

        public Weather()
        {
            Web web = new();
            GetWeather(web.GetGlobalCoordinates());
        }

        public SingleWeather GetCurrentWeather()
        {
            return CurrnetWeather;
        }
        public WeatherInfo GetWholeForecast()
        {
            return WholeForecast;
        }

        private void GetWeather((double lat, double lon) geoLocation)
        {
            var weatherUrlStart = "http://www.7timer.info/bin/api.pl?";
            StringBuilder stringBuilder = new(weatherUrlStart);
            stringBuilder.Append("lon=" + geoLocation.lon + "&lat=" + geoLocation.lat);
            stringBuilder.Append("&product=civil&output=json");
            string respond = "";
            WebRequest request = WebRequest.Create(stringBuilder.ToString());
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new(response.GetResponseStream()))
            {
                respond = stream.ReadToEnd();
            }
            string info = new WebClient().DownloadString(stringBuilder.ToString());

            SetWholeForecast(JsonConvert.DeserializeObject<WeatherInfo>(info));
            SetCurrentWeather(FindCurrnetWeather());
        }

        private SingleWeather FindCurrnetWeather()
        {
            return WholeForecast.Dataseries.Single(x => x.Timepoint == "9");
        }

        private void SetWholeForecast(WeatherInfo weatherInfo)
        {
            WholeForecast = weatherInfo;
        }
        private void SetCurrentWeather(SingleWeather singleWeather)
        {
            CurrnetWeather = singleWeather;
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
