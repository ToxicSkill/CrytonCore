using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
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
            Task.Run(() => UpdateInternetStatus()).ConfigureAwait(true);
        }
        internal string GetInternetStatusString()
        {
            return InternetStatus ? "Connected to internet" : "Not connected to internet";
        }
        internal async Task<bool> GetInternetStatus()
        {
            return await CheckForInternetConnection() || await CheckForInternetConnectionSec();
        }
        internal async Task UpdateInternetStatus()
        {
            InternetStatus = await GetInternetStatus();
        }
        internal SolidColorBrush GetInternetStatusColor()
        {
            return InternetStatus ? new SolidColorBrush(Colors.YellowGreen) : new SolidColorBrush(Colors.Red);
        }
        private async Task<bool> CheckForInternetConnection()
        {
            return await Task.Run(() =>
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
            });
        }
        private async Task<bool> CheckForInternetConnectionSec()
        {
            return await Task.Run(() =>
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
            });
        }
    }
}
