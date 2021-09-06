using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrytonCore.ViewModel
{
    public class WelcomePageViewModel : NotificationClass
    {
        private readonly DispatcherTimer _liveTime = new()
        {
            Interval = TimeSpan.FromSeconds(SecondsDelay)
        };
        private readonly DispatcherTimer _internetTime = new()
        {
            Interval = TimeSpan.FromMinutes(MinutesDelay)
        };

        private readonly TimeDate _actualTimeDate = new();
        private readonly InternetConnection _internetConnection = new();
        private SolidColorBrush _internetColorDiode = new();

        private string _currentTime;
        private string _currentDay;
        private string _toolTip;

        private const int SecondsDelay = 1;
        private const int MinutesDelay = 1;
        public WelcomePageViewModel()
        {
            UpdateInternetStatus();
            UpdateTime();

            _liveTime.Tick += TimeTimer_Tick;
            _liveTime.Start();
            _internetTime.Tick += InternetTimer_Tick;
            _internetTime.Start();
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
        public Transform SubtitleTransform => new ScaleTransform(0.9, 1);
        private void InternetTimer_Tick(object sender, EventArgs e) => UpdateInternetStatus();
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
    }
}
