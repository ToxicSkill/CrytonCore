﻿
using System.Web;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for PasswordManagerPage.xaml
    /// </summary>
    public partial class PasswordManagerPage : Page
    {
        public PasswordManagerPage()
        {
            InitializeComponent();
            var geoLocation = GetUserCountryByIp(GetIPAddressPublic());
            GetWeather(geoLocation);
        }

        private static void GetWeather((double lat, double lon) geoLocation)
        {
            var weatherUrlStart = "http://www.7timer.info/bin/api.pl?";
            StringBuilder stringBuilder = new(weatherUrlStart);
            stringBuilder.Append("lon=" + geoLocation.lon + "&lat=" + geoLocation.lat);
            stringBuilder.Append("&product=astro&output=json");
            string respond = "";
            WebRequest request = WebRequest.Create(stringBuilder.ToString());
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new(response.GetResponseStream()))
            {
                respond = stream.ReadToEnd();
            }

            string wet = "[\n\t{\n\t\t\"timepoint\" : 3,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 5,\n\t\t\"lifted_index\" : 10,\n\t\t\"rh2m\" : 13,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 7,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 6,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 4,\n\t\t\"lifted_index\" : 10,\n\t\t\"rh2m\" : 12,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 8,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 9,\n\t\t\"cloudcover\" : 8,\n\t\t\"seeing\" : 3,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 10,\n\t\t\"rh2m\" : 9,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 12,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 12,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 3,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 6,\n\t\t\"rh2m\" : 7,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 14,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 15,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 4,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 6,\n\t\t\"rh2m\" : 8,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 13,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 18,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 4,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 6,\n\t\t\"rh2m\" : 9,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 12,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 21,\n\t\t\"cloudcover\" : 7,\n\t\t\"seeing\" : 4,\n\t\t\"transparency\" : 5,\n\t\t\"lifted_index\" : 10,\n\t\t\"rh2m\" : 12,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"NW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 8,\n\t\t\"prec_type\" : \"rain\"\n\t},\n\t{\n\t\t\"timepoint\" : 24,\n\t\t\"cloudcover\" : 5,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 8,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 14,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 6,\n\t\t\"prec_type\" : \"rain\"\n\t},\n\t{\n\t\t\"timepoint\" : 27,\n\t\t\"cloudcover\" : 4,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 8,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 14,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 5,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 30,\n\t\t\"cloudcover\" : 5,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 6,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 14,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 4,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 33,\n\t\t\"cloudcover\" : 5,\n\t\t\"seeing\" : 3,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 8,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"NW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 9,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 36,\n\t\t\"cloudcover\" : 5,\n\t\t\"seeing\" : 3,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 6,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"NW\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 10,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 39,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 3,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 8,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 9,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 42,\n\t\t\"cloudcover\" : 8,\n\t\t\"seeing\" : 6,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 11,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 6,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 45,\n\t\t\"cloudcover\" : 7,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 10,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 7,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 48,\n\t\t\"cloudcover\" : 7,\n\t\t\"seeing\" : 6,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 11,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 5,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 51,\n\t\t\"cloudcover\" : 5,\n\t\t\"seeing\" : 6,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 12,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 4,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 54,\n\t\t\"cloudcover\" : 7,\n\t\t\"seeing\" : 6,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 11,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 4,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 57,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 3,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 7,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"SW\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 8,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 60,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 3,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 5,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 11,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 63,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 7,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 10,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 66,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 6,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 9,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 8,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 69,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 6,\n\t\t\"transparency\" : 3,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 10,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 2\n\t\t},\n\t\t\"temp2m\" : 7,\n\t\t\"prec_type\" : \"none\"\n\t},\n\t{\n\t\t\"timepoint\" : 72,\n\t\t\"cloudcover\" : 9,\n\t\t\"seeing\" : 5,\n\t\t\"transparency\" : 2,\n\t\t\"lifted_index\" : 15,\n\t\t\"rh2m\" : 9,\n\t\t\"wind10m\" : {\n\t\t\t\"direction\" : \"W\",\n\t\t\t\"speed\" : 3\n\t\t},\n\t\t\"temp2m\" : 8,\n\t\t\"prec_type\" : \"none\"\n\t}\n\t]";
            string info = new WebClient().DownloadString(stringBuilder.ToString());
            var weatherInfo = JsonConvert.DeserializeObject<IEnumerable<DataSeries>>(wet);
        }

        private static string GetIPAddressPublic()
        {
            string address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new (response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return address;
        }
        public static (double lat, double lon) GetUserCountryByIp(string ip)
        {
            IpInfo ipInfo = new();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new(ipInfo.Country);
                var gelocString = ipInfo.Loc.Split(',');
                double resOne, resTwo;
                double.TryParse(gelocString[0], out resOne);
                double.TryParse(gelocString[1], out resTwo);
                return (lat: resOne, lon: resTwo);
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return (-1, -1);
        }
        public class IpInfo
        {
            [JsonProperty("ip")]
            public string Ip { get; set; }

            [JsonProperty("hostname")]
            public string Hostname { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("loc")]
            public string Loc { get; set; }

            [JsonProperty("org")]
            public string Org { get; set; }

            [JsonProperty("postal")]
            public string Postal { get; set; }
        }
        public class WeatherInfo
        {

            [JsonProperty("product")]
            public string Product { get; set; }

            [JsonProperty("init")]
            public string Init { get; set; }

            [JsonProperty("dataseries")]
            public DataSeries Dataseries { get; set; }
        }

        public class DataSeries
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
            //private void Button_Click(object sender, RoutedEventArgs e)
            //{
            //    Button MyControl1 = new();
            //    MyControl1.Content = count.ToString();
            //    MyControl1.Name = "Button" + count.ToString();
            //    MyControl1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFE));
            //    MyControl1.Foreground = Brushes.Violet;
            //    Grid.SetColumn(MyControl1, j);
            //    Grid.SetRow(MyControl1, i);
            //    j++;
            //    if (j == 6)
            //    {
            //        j = 0;
            //        i++;
            //    }
            //    count++;

            //}
        }
}
