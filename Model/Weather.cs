﻿using Newtonsoft.Json;
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
        public SunInfo TodaySunInfo { get; set; }
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
            GetSunraiseSunset(geoLocation);
        }

        private void GetSunraiseSunset((double lat, double lon) geoLocation)
        {
            //var y = (2 * Math.PI / 365) * (DateTime.Now.DayOfYear - 1 + (DateTime.Now.Hour - 12) / 24);
            //var eqtime = 229.18 * (0.000075 + 0.001868 * Math.Cos(y) - 0.032077 * Math.Sin(y) - .014615 * Math.Cos(2 * y) - 0.040849 * Math.Sin(2 * y));
            //var declin = 0.06918 - 0.399912 * Math.Cos(y) + 0.070257 * Math.Sin(y) - 0.006758 * Math.Cos(2 * y) + 0.000907 * Math.Sin(2 * y) - 0.002687 * Math.Cos(3 * y) + 0.00148 * Math.Sin(3 * y);

            //var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);
            //var timeOffset = eqtime - 4 * geoLocation.lon + 60 * utcOffset.Hours;
            //var tst = DateTime.Now.Hour * 60 + DateTime.Now.Minute + timeOffset;
            //var ha1 = tst / 4 - 180;
            //var cosPhi = Math.Sin(geoLocation.lat) * Math.Sin(declin) + Math.Cos(geoLocation.lat) * Math.Cos(declin) * Math.Cos(ha1);
            //var cos180Theta =-(Math.Sin(geoLocation.lat) * Math.Cos(cosPhi) - Math.Sin(declin))/(Math.Cos(geoLocation.lat)  * Math.Sin(cosPhi));
            //var ha = Math.Acos(Math.Cos(90.833 / (Math.Cos(geoLocation.lat) * Math.Cos(declin))) - Math.Tan(geoLocation.lat) * Math.Tan(declin));
            //var sunrise = (720 + 4 * (geoLocation.lat - ha) - eqtime) / 60;
            //var snoon = (720 + 4 * geoLocation.lat - eqtime) / 60;

            var sunsetSunriseStart = "https://api.sunrise-sunset.org/json?";
            StringBuilder stringBuilder = new(sunsetSunriseStart);
            _ = stringBuilder.Append("lat=" + geoLocation.lat + "&lng=" + geoLocation.lon);
            _ = stringBuilder.Append("&date=today");
            string info = new WebClient().DownloadString(stringBuilder.ToString());
            SetTodaySunInfo(JsonConvert.DeserializeObject<SunInfo>(info));
            GetCurrentSunrise();
            GetCurrentSunset();
        }

        private void SetTodaySunInfo(SunInfo sunInfo)
        {
            TodaySunInfo = sunInfo;
        }

        private void SetCurrentWeatherIcon()
        {
            var cloundIndex = int.Parse(ActualWeather.Cloudcover);
            var liftedIndex = int.Parse(ActualWeather.LiftedIndex);

            switch (ActualWeather.PrecType.ToString())
            {
                case "rain":
                    if (liftedIndex < -5)
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
                    if (cloundIndex == 9 && liftedIndex > -5)
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

        public string GetCurrentSunrise()
        {
            var sunrise = TodaySunInfo.Details.Sunrise;
            var date = sunrise.ToString().Substring(0, sunrise.Length - 3);
            var time = date.Split(':');
            var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);
            var sunHour = int.Parse(time[0], System.Globalization.NumberStyles.Integer) + utcOffset.Hours;
            if (sunrise.EndsWith("PM"))
                sunHour += 12;
            var sunriseFinal = string.Concat(sunHour.ToString(), ":", time[1]);
            return sunriseFinal;
        }

        public string GetCurrentSunset()
        {
            var sunset = TodaySunInfo.Details.Sunset;
            var date = sunset.ToString().Substring(0, sunset.Length - 3);
            var time = date.Split(':');
            var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);
            var sunHour = int.Parse(time[0], System.Globalization.NumberStyles.Integer) + utcOffset.Hours;
            if (sunset.EndsWith("PM"))
                sunHour += 12;
            var sunsetFinal = string.Concat(sunHour.ToString(), ":", time[1]);
            return sunsetFinal;
        }

        private SingleWeather FindCurrnetWeather()
        {
            var initDate = WholeForecast.Init;
            DateTime firstDate = DateTime.Now;
            DateTime secondDate = new(
                year: int.Parse(initDate.Substring(0, 4), System.Globalization.NumberStyles.Integer),
                month: int.Parse(initDate.Substring(4, 2), System.Globalization.NumberStyles.Integer),
                day: int.Parse(initDate.Substring(6, 2), System.Globalization.NumberStyles.Integer),
                hour: int.Parse(initDate.Substring(8, 2), System.Globalization.NumberStyles.Integer),
                minute: 00,
                second: 00);

            TimeSpan diff = secondDate.Subtract(firstDate);
            var diffHours = diff.TotalHours;
            var index = Math.Floor(Math.Abs(diffHours) / 3);
            return WholeForecast.Dataseries.Single(x => x.Timepoint == (index * 3).ToString());
        }

        private void SetWholeForecast(WeatherInfo weatherInfo)
        {
            WholeForecast = weatherInfo;
        }
        private void SetCurrentWeather(SingleWeather singleWeather)
        {
            ActualWeather = singleWeather;
        }

        public class SunInfo
        {
            [JsonProperty("results")]
            public SunDetails Details { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public class SunDetails
        {
            [JsonProperty("sunrise")]
            public string Sunrise { get; set; }

            [JsonProperty("sunset")]
            public string Sunset { get; set; }

            [JsonProperty("solar_noon")]
            public string SolarNoon { get; set; }

            [JsonProperty("day_length")]
            public string DayLength { get; set; }

            [JsonProperty("civil_twilight_begin")]
            public string CivilTwilightBegin { get; set; }

            [JsonProperty("civil_twilight_end")]
            public string CivilTwilightEnd { get; set; }

            [JsonProperty("nautical_twilight_begin")]
            public string NauticalTwilightBegin { get; set; }

            [JsonProperty("nautical_twilight_end")]
            public string NauticalTwilightEnd { get; set; }

            [JsonProperty("astronomical_twilight_begin")]
            public string AstronomicalTwilightBegin { get; set; }

            [JsonProperty("astronomical_twilight_end")]
            public string AstronomicalTwilightEnd { get; set; }
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
