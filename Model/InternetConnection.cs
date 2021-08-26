using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace CrytonCore.Model
{
    public class InternetConnection
    {
        private static readonly string _pingString = "http://www.google.com";
        public string InternetStatusString { get; set; }
        public bool InternetStatus { get; set; }
        public SolidColorBrush InternetSatusColor { get; set; }
        public InternetConnection()
        {
            SetInternetStatus();
        }
        internal string GetInternetStatusString()
        {
            return InternetStatus ? "Connected to internet" : "Not connected to internet";
        }
        internal bool GetInternetStatus()
        {
            return CheckForInternetConnection() || CheckForInternetConnectionSec();
        }
        internal void SetInternetStatus()
        {
            InternetStatus = GetInternetStatus();
        }
        internal SolidColorBrush GetInternetStatusColor()
        {
            return InternetStatus ? new SolidColorBrush(Colors.YellowGreen) : new SolidColorBrush(Colors.Red);
        }
        private static bool CheckForInternetConnection()
        {
            try
            {
                using WebClient client = new();
                using System.IO.Stream stream = client.OpenRead(_pingString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static bool CheckForInternetConnectionSec()
        {
            try
            {
                Ping myPing = new();
                string host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
