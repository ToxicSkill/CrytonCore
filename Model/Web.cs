using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace CrytonCore.Model
{
    public class Web
    {
        public JsonWeb WebInfo { get; set; }

        public Web()
        {
            GetIPAddressPublic();
        }

        public JsonWeb GetAllWebInfo()
        {
            return WebInfo;
        }

        public (double latitude, double longnitude) GetGlobalCoordinates()
        {
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + WebInfo.Ip);
                var gelocString = WebInfo.Loc.Split(',');
                var resOne = double.Parse(gelocString[0], CultureInfo.InvariantCulture);
                var resTwo = double.Parse(gelocString[1], CultureInfo.InvariantCulture);
                return (latitude: resOne, longnitude: resTwo);
            }
            catch (Exception)
            {
                return (latitude: -1, longnitude: -1);
            }
        }

        public string GetCurrentCity()
        {
            return WebInfo.City;
        }
        public string GetCurrentCountry()
        {
            return WebInfo.Country;
        }
        public string GetCurrentRegion()
        {
            return WebInfo.Region;
        }
        public string GetOrganization()
        {
            return WebInfo.Org;
        }
        public string GetPostalCode()
        {
            return WebInfo.Postal;
        }
        public string GetHostname()
        {
            return WebInfo.Hostname;
        }

        private void GetIPAddressPublic()
        {
            string address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new(response.GetResponseStream()))
                address = stream.ReadToEnd();
            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);
            string info = new WebClient().DownloadString("http://ipinfo.io/" + address);
            WebInfo = JsonConvert.DeserializeObject<JsonWeb>(info);
        }

        public class JsonWeb
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
    }
}
