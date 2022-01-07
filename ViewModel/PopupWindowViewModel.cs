using CrytonCore.Enums;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Visibility;

namespace CrytonCore.ViewModel
{
    internal class PopupWindowViewModel : NotificationClass
    {
        public string Title { get; init; }
        public string Message { get; init; }
        public bool Result { get; set; }


        #region Visibilities

        public Visibility InformationVisibility { get; private set; } = Hidden;
        public Visibility WarningVisibility { get; private set; } = Hidden;
        public Visibility ErrorVisibility { get; private set; } = Hidden;
        public Visibility FatalErrorVisibility { get; private set; } = Hidden;

        #endregion

        public Action CloseAction { get; set; }

        public PopupWindowViewModel() : this(null) { }

        public PopupWindowViewModel(ILogger logger)
        {
            if (logger == null)
            {
                Title = Enums.ELogger.EnumToString(Enums.ELogger.Level.Error);
            }
            Title = logger.GetLevelName();
            Message = logger.GetMessage();
            SetVisibilities(logger.GetLevel());
        }

        private void SetVisibilities(ELogger.Level level)
        {
            switch (level)
            {
                case ELogger.Level.Information:
                    InformationVisibility = Visible;
                    break;
                case ELogger.Level.Warning:
                    WarningVisibility = Visible;
                    OnPropertyChanged(nameof(WarningVisibility));
                    break;
                case ELogger.Level.Error:
                    ErrorVisibility = Visible;
                    OnPropertyChanged(nameof(ErrorVisibility));
                    break;
                case ELogger.Level.Fatal:
                    FatalErrorVisibility = Visible;
                    OnPropertyChanged(nameof(FatalErrorVisibility));
                    break;
                default:
                    InformationVisibility = Visible;
                    break;
            }
            OnPropertyChanged(nameof(InformationVisibility));
        }

        public RelayCommand Ok => new(OkCommand, true);

        private void OkCommand() => CloseAction();
    }
}
