using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Windows.Media;
using System.Windows.Threading;
using static CrytonCore.Model.Weather;

namespace CrytonCore.ViewModel
{
    public class WelcomePageViewModel : NotificationClass
    {
        private readonly DispatcherTimer _secondTime = new()
        {
            Interval = TimeSpan.FromSeconds(SecondsDelay)
        };
        private readonly DispatcherTimer _minuteTime = new()
        {
            Interval = TimeSpan.FromMinutes(MinutesDelay)
        };

        private readonly TimeDate _actualTimeDate = new();
        private readonly InternetConnection _internetConnection = new();
        private SolidColorBrush _internetColorDiode = new();
        private Weather _weather = new();
        private SingleWeather _actualWeather = new();
        private Web _web = new();

        private string _currentTime;
        private string _currentDay;
        private string _toolTip;
        private string _actualTemperature;
        private string _actualHumidity;
        private string _actualWind;
        private string _actualWeatherIcon;
        private string _actualCity;
        private string _actualRegion;
        private string _actualCountry;

        private const int SecondsDelay = 1;
        private const int MinutesDelay = 1;
        public WelcomePageViewModel()
        {
            UpdateInternetStatus();
            UpdateTime();
            UpdateWeatherStatus();
            UpdateWebStatus();

            _secondTime.Tick += TimeTimer_Tick;
            _secondTime.Start();
            _minuteTime.Tick += InternetTimer_Tick;
            _minuteTime.Start();
        }

        public string ActualCity
        { 
            get => _actualCity;
            set
            {
                _actualCity = value;
                OnPropertyChanged(nameof(ActualCity));
            }
        }
        public string ActualRegion
        { 
            get => _actualRegion;
            set
            {
                _actualRegion = value;
                OnPropertyChanged(nameof(ActualRegion));
            }
        }
        public string ActualCountry
        { 
            get => _actualCountry;
            set
            {
                _actualCountry = value;
                OnPropertyChanged(nameof(ActualCountry));
            }
        }
        public string ActualTemperature
        {
            get => _actualTemperature;
            set
            {
                _actualTemperature = value;
                OnPropertyChanged(nameof(ActualTemperature));
            }
        }
        public string ActualHumidity
        {
            get => _actualHumidity;
            set
            {
                _actualHumidity = value;
                OnPropertyChanged(nameof(ActualHumidity));
            }
        }
        public string ActualWind
        {
            get => _actualWind;
            set
            {
                _actualWind = value;
                OnPropertyChanged(nameof(ActualWind));
            }
        }
        public string ActualWeatherIcon
        {
            get => _actualWeatherIcon;
            set
            {
                _actualWeatherIcon = value;
                OnPropertyChanged(nameof(ActualWeatherIcon));
            }
        }


        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        public string CurrentDay
        {
            get => _currentDay;
            set
            {
                _currentDay = value;
                OnPropertyChanged(nameof(CurrentDay));
            }
        }
        public string ToolTip
        {
            get => _toolTip;
            set
            {
                _toolTip = value;
                OnPropertyChanged(nameof(ToolTip));
            }
        }
        public SolidColorBrush FillDiode
        {
            get => _internetColorDiode;
            set
            {
                _internetColorDiode = value;
                OnPropertyChanged(nameof(FillDiode));
            }
        }
        public static Transform SubtitleTransform => new ScaleTransform(0.9, 1);
        private void InternetTimer_Tick(object sender, EventArgs e)
        {
            UpdateInternetStatus();
            UpdateWeatherStatus();
            UpdateWebStatus();
        }
        private void TimeTimer_Tick(object sender, EventArgs e) => UpdateTime();
        private void UpdateTime()
        {
            CurrentTime = _actualTimeDate.GetCurrentTime();
            CurrentDay = _actualTimeDate.GetCurrentDay();
        }
        private void UpdateInternetStatus()
        {
            _internetConnection.SetInternetStatus();
            FillDiode = _internetConnection.GetInternetStatusColor();
            ToolTip = _internetConnection.GetInternetStatusString();
        }
        private void UpdateWeatherStatus()
        {
            _actualWeather = _weather.GetActualWeather();
            ActualTemperature = _actualWeather.Temp.ToString() + "ºC";
            ActualHumidity = _actualWeather.Rh2m.ToString();
            ActualWind = _actualWeather.Wind10m.Direction.ToString();
            ActualWeatherIcon = _weather.GetActualWeatherIcon();
        }
        private void UpdateWebStatus()
        {
            ActualCity = _web.GetCurrentCity();
            ActualCountry = _web.GetCurrentCountry();
            ActualRegion = _web.GetCurrentRegion();
        }
    }
}
