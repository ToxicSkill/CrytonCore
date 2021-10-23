using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using static CrytonCore.Model.Weather;

namespace CrytonCore.ViewModel
{
    public class WelcomePageViewModel : NotificationClass
    {
        private readonly DispatcherTimer _timeTime = new()
        {
            Interval = TimeSpan.FromSeconds(SecondsDelay)
        };
        private readonly DispatcherTimer _webTime = new()
        {
            Interval = TimeSpan.FromMinutes(MinutesDelay)
        };
        private readonly DispatcherTimer _weatherTime = new()
        {
            Interval = TimeSpan.FromMinutes(MinutesDelay)
        };
        private readonly DispatcherTimer _firstRunDelayer = new()
        {
            Interval = TimeSpan.FromSeconds(SecondsDelay)
        };

        private readonly TimeDate _actualTimeDate = new();
        private readonly InternetConnection _internetConnection = new();
        private SolidColorBrush _internetColorDiode = new();
        private readonly Weather _weather = new();
        private readonly Web _web = new();

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
        private string _sunrise;
        private string _sunset;

        private const int SecondsDelay = 1;
        private const int MinutesDelay = 1;

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
        public string Sunrise
        {
            get => _sunrise;
            set
            {
                _sunrise = value;
                OnPropertyChanged(nameof(Sunrise));
            }
        }
        public string Sunset
        {
            get => _sunset;
            set
            {
                _sunset = value;
                OnPropertyChanged(nameof(Sunset));
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
        
        private async Task TimeTimer_Tick(object sender, EventArgs e) => await UpdateTime();

        private async Task UpdateTime()
        {
            CurrentTime = _actualTimeDate.GetCurrentTime();
            CurrentDay = _actualTimeDate.GetCurrentDay();
            await Thread;
        }

        private async Task WeatherInfoTimer_Tick(object s, EventArgs e) => await UpdateWeatherStatus();

        private async Task WebInfoTimer_Tick(object sender, EventArgs e) => await UpdateWebStatus();

        private async Task UpdateWebStatus()
        {
            try
            {
                await _internetConnection.UpdateInternetStatus();
            }
            catch (Exception)
            {
            }
            finally
            {
                await Thread;

                ActualCity = _web.GetCurrentCity();
                ActualCountry = _web.GetCurrentCountry();
                ActualRegion = _web.GetCurrentRegion();
                FillDiode = _internetConnection.GetInternetStatusColor();
                ToolTip = _internetConnection.GetInternetStatusString();
            }
        }
        private async Task UpdateWeatherStatus()
        {
            try
            {
                await _weather.UpdateWeather();
            }
            catch (Exception)
            {
            }
            finally
            {
                await Thread;
                ActualTemperature = _weather.GetActualTemperature();
                ActualHumidity = _weather.GetActualHumidity();
                ActualWind = _weather.GetActualWind();
                ActualWeatherIcon = _weather.GetActualWeatherIcon();
                Sunrise = _weather.GetCurrentSunrise();
                Sunset = _weather.GetCurrentSunset();
            }
        }
        public WelcomePageViewModel()
        {
            _firstRunDelayer.Tick += (s, e) => Task.Run(() => FirstRunUpdates(s, e));
            _firstRunDelayer.Start();
            _timeTime.Tick += (s, e) => Task.Run(() => TimeTimer_Tick(s, e));
            _timeTime.Start();
            _webTime.Tick += (s, e) => Task.Run(() => WebInfoTimer_Tick(s, e));
            _webTime.Start();
            _weatherTime.Tick += (s, e) => Task.Run(() => WeatherInfoTimer_Tick(s, e));
            _weatherTime.Start();
        }


        private async Task FirstRunUpdates(object s, EventArgs e)
        {
            await UpdateWebStatus();
            _firstRunDelayer.Stop();
        }

        public static DispatcherAwaiter Thread => new();

        public struct DispatcherAwaiter : INotifyCompletion
        {
            public bool IsCompleted => Application.Current.Dispatcher.CheckAccess();

            public void OnCompleted(Action continuation) => Application.Current.Dispatcher.Invoke(continuation);

            public void GetResult() { }

            public DispatcherAwaiter GetAwaiter()
            {
                return this;
            }
        }
    }
}
