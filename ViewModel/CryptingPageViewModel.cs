using CrytonCore.File;
using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using CrytonCore.Model;
using CrytonCore.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using static CrytonCore.Enums.ETypesOfCrypting;

namespace CrytonCore.ViewModel
{
    public class CryptingPageViewModel : NotificationClass, IFileDragDropTarget
    {
        public SimpleFile SFile { get; set; } = new SimpleFile();
        public bool IsRunning { get; set; }

        private readonly Crypting _crypting = new();
        private Dictionary<string, object> GridsDict;
        private readonly Dictionary<TypesOfCrypting, IHelpersInterface> _crytpingSettings = new();

        private readonly CancellationTokenSource _cts = new();
        private string _selectedCryptingMethod;
        private const double BarExtension4KReady = 3840;
        private bool _moveDetails;
        private double _opacity = 0.6;
        private double _opacityHelp = 1;
        private int _progressDivisor = 1;
        private static readonly int SecondsDelay = 2;

        private readonly DispatcherTimer _infoTime = new()
        {
            Interval = TimeSpan.FromSeconds(SecondsDelay)
        };

        private readonly IDialogService _dialogService;

        public CryptingPageViewModel()
        {
            _dialogService = new DialogService();
            _dialogService.Register<DialogViewModel, DialogWindow>();

            InitializeCryptingSettings();

            CryptingMethodsCollection = _crypting.Get();
            _selectedCryptingMethod = _crypting.GetCryptingMethodName();

            _moveDetails = false;

            _infoTime.Tick += InfoTimer_Tick;

            InitializeGridsDictionary();
            SetActualGridVisibility();
        }

        private void InitializeCryptingSettings()
        {
            _crytpingSettings.Add(TypesOfCrypting.RSA, new RSAHelper());
            _crytpingSettings.Add(TypesOfCrypting.CESAR, new CesarHelper());

            RsaCollection = _crytpingSettings[TypesOfCrypting.RSA].GetFirst();
            SelectedRsaCollection = _crytpingSettings[TypesOfCrypting.RSA].GetSelectedItemFirst();

            CesarCollection = _crytpingSettings[TypesOfCrypting.CESAR].GetFirst();
            SelectedCesarCollection = _crytpingSettings[TypesOfCrypting.CESAR].GetSelectedItemFirst();
        }

        private void InitializeGridsDictionary()
        {
            GridsDict = new Dictionary<string, object>
            {
                { EnumToString(TypesOfCrypting.CESAR), typeof(CryptingPageViewModel).GetProperty(nameof(CesarGridVisibility)) },
                { EnumToString(TypesOfCrypting.RSA), typeof(CryptingPageViewModel).GetProperty(nameof(RsaGridVisibility)) }
            };
        }

        private void SetActualGridVisibility()
        {
            foreach (KeyValuePair<string, object> item in GridsDict)
            {
                (item.Value as PropertyInfo).SetValue(this, Visibility.Hidden);
            }
            try
            {
                object property = GridsDict[_crypting.GetCryptingMethodName()];
                string[] propertyName = property.ToString()?.Split(' ');
                (property as PropertyInfo)?.SetValue(this, Visibility.Visible);
                if (propertyName != null) OnPropertyChanged(propertyName[1]);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async void OnFileDropAsync(string[] filePaths)
        {
            _ = await LoadFile(filePaths[0]);
        }

        public RelayAsyncCommand<object> LoadFileViaDialog => new(LoadFileViaDialogCommand);

        public async Task<bool> LoadFileViaDialogCommand(object o)
        {
            WindowDialogs.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.EnumToString(Enums.EDialogFilters.DialogFilters.All),
                Multiselect = false,
                Title = "Open file"
            });
            var dialogResult = openDialog.RunDialog();
            return dialogResult is not null ? await LoadFile(dialogResult.First()) : await Task.Run(() => false);
        }

        private async Task<bool> LoadFile(string fileName)
        {
            var loadingFileResult = false;

            try
            {
                loadingFileResult = await _crypting.LoadFile(fileName).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignored
            }

            if (loadingFileResult)
            {
                UpdatePageContent();
                VisibilityShowCommand();
                MoveDetails = true;
                MoveBars = true;
            }
            return loadingFileResult;
        }
        public RelayCommand SaveFile => new(SaveFileCommand, true);

        private async void SaveFileCommand()
        {
            Progress<string> progressIndicator = new();
            progressIndicator.ProgressChanged += StringReportProgress;

            try
            {
                VisibilityProgress = Visibility.Visible;
                _ = await _crypting.SaveFile(progressIndicator);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                _infoTime.Start();
            }
        }
        private void StringReportProgress(object sender, string e) => ProcessText = e;


        public RelayCommand CipherAction => new(CipherActionCommand, true);

        private async void CipherActionCommand()
        {
            // if (!CanExecute())
            //  return;
            if (SFile == null || SFile.ChunkSize == 0) return;
            if (SFile.Status)
            {
                CheckConditions();
            }
            Progress<int> progressIndicator = new();
            BeforeCipherAction();

            // Task<bool> runningTask = crypting.Crypt(progressIndicator);
            //RunningTasks.Add(runningTask);

            try
            {
                progressIndicator.ProgressChanged += PercentageReportProgress;
                var result = await _crypting.Crypt(progressIndicator, _cts.Token);
                SFile = _crypting.UpdateSimpleFile();
                if (SFile.Status && result)
                {
                    UpdatePageContent();
                }
                else if (!SFile.Status && !result)
                {
                    UpdatePageContent();
                }
            }
            catch (OperationCanceledException)
            {
                ProcessText = "Cancelled";
            }
            finally
            {
                //RunningTasks.Remove(runningTask);
                AfterCipherAction();
            }

        }

        private void CheckConditions()
        {
            if (_crypting.GetCryptingMethodName().ToUpper() == EnumToString(TypesOfCrypting.RSA).ToUpper())
            {
                BlurWindow();
                Views.InputCryptingWindow dlg = new();
                _ = dlg.ShowDialog();
                UnblurWindow();
            }
        }

        private void BeforeCipherAction()
        {
            IsRunning = true;
            VisibilityProcess = Visibility.Visible;
            _progressDivisor = SFile.ChunkSize;
            //  cryptingButtonEnabled = false;
            //OnPropertyChanged("CryptingButtonEnabled");
        }
        private void AfterCipherAction()
        {
            VisibilityProcess = Visibility.Hidden;
            ProgressText = "";
            IsRunning = false;
            _progressDivisor = 1;
            //OnPropertyChanged("CryptingButtonEnabled");
        }
        public RelayCommand CancelAction => new(CancelActionCommand, true);

        private void CancelActionCommand()
        {
            _cts.Cancel();
        }
        private void PercentageReportProgress(object sender, int value)
        {
            SetProgressText((value * 100 / _progressDivisor) + "%");
        }
        private void InfoTimer_Tick(object sender, EventArgs e) => ClearProcessInfo();

        private void ClearProcessInfo()
        {
            _infoTime.Stop();
            ProgressText = "";
            ProcessText = "";
            VisibilityProgress = Visibility.Hidden;
        }

        private string _progressText;

        public string GetProgressText()
        {
            return _progressText;
        }
        public string ProgressText
        {
            get => _progressText;
            set
            {
                _progressText = value;
                OnPropertyChanged(nameof(ProgressText));
            }
        }

        private void SetProgressText(string value)
        {
            ProgressText = value;
        }
        private string _processText;
        public string ProcessText
        {
            get => _processText;
            set
            {
                _processText = value;
                OnPropertyChanged(nameof(ProcessText));
            }
        }
        public string SelectedCryptingMethod
        {
            get => _selectedCryptingMethod;
            set
            {
                _selectedCryptingMethod = value;
                _crypting.SetCryptingMethod(_selectedCryptingMethod);
                SetActualGridVisibility();
                OnPropertyChanged(nameof(SelectedCryptingMethod));
            }
        }
        public ObservableCollection<string> CryptingMethodsCollection { get; }

        private string _selectedRsaCollection;
        public string SelectedRsaCollection
        {
            get => _crytpingSettings[TypesOfCrypting.RSA].GetSelectedItemFirst();
            set
            {
                _selectedRsaCollection = value;
                // crytpingSettings[(int)Enums.Enumerates.TypesOfCrypting.RSA].SetSelectedItemFirst( selectedRsaCollection);
                _crytpingSettings[TypesOfCrypting.RSA].SetSelectedItemFirst(_selectedRsaCollection);
                OnPropertyChanged(nameof(SelectedRsaCollection));
            }
        }
        public ObservableCollection<string> RsaCollection { get; set; }

        private string _selectedCesarCollection;
        public string SelectedCesarCollection
        {
            get => _crytpingSettings[TypesOfCrypting.CESAR].GetSelectedItemFirst();
            set
            {
                _selectedCesarCollection = value;
                // crytpingSettings[(int)Enums.Enumerates.TypesOfCrypting.RSA].SetSelectedItemFirst( selectedRsaCollection);
                _crytpingSettings[TypesOfCrypting.CESAR].SetSelectedItemFirst(_selectedCesarCollection);
                OnPropertyChanged(nameof(SelectedCesarCollection));
            }
        }
        public ObservableCollection<string> CesarCollection { get; set; }

        public bool MoveDetails
        {
            get => _moveDetails;
            set
            {
                _moveDetails = value;
                OnPropertyChanged(nameof(MoveDetails));
            }
        }
        public RelayCommand AnimationRectangle => new(MoveDetailsWindow, true);

        private void MoveDetailsWindow()
        {
            MoveDetails = !MoveDetails;
        }

        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }

        public RelayCommand ChangeOpacityFull => new(OpacityChangeFull, true);
        private void OpacityChangeFull() => Opacity = 1;
        public RelayCommand ChangeOpacityPartial => new(OpacityChangePartial, true);
        private void OpacityChangePartial() => Opacity = 0.6;
        public double OpacityHelp
        {
            get => _opacityHelp;
            set
            {
                _opacityHelp = value;
                OnPropertyChanged(nameof(OpacityHelp));
            }
        }

        public RelayCommand ChangeHelpOpacityFull => new(OpacityHelpChangeFull, true);
        private void OpacityHelpChangeFull() => OpacityHelp = 1;
        public RelayCommand ChangeHelpOpacityPartial => new(OpacityHelpChangePartial, true);
        private void OpacityHelpChangePartial() => OpacityHelp = 0.6;

        private Visibility _visibilityHidden = Visibility.Hidden;
        private Visibility _visibilityShowed = Visibility.Visible;
        private Visibility _visibilityProgress = Visibility.Hidden;
        private Visibility _visibilityProcess = Visibility.Hidden;
        private Visibility _openPadlockVisibility = Visibility.Hidden;
        private Visibility _closePadlockVisibility = Visibility.Hidden;

        public Visibility OpenPadlockVisibility { get => _openPadlockVisibility; set { _openPadlockVisibility = value; OnPropertyChanged(nameof(OpenPadlockVisibility)); } }
        public Visibility ClosePadlockVisibility { get => _closePadlockVisibility; set { _closePadlockVisibility = value; OnPropertyChanged(nameof(ClosePadlockVisibility)); } }
        public Visibility VisibilityDefaultAsShowed
        {
            get => _visibilityShowed;
            set
            {
                _visibilityShowed = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsShowed));
            }
        }
        public Visibility VisibilityDefaultAsHidden
        {
            get => _visibilityHidden;
            set
            {
                _visibilityHidden = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsHidden));
            }
        }
        public Visibility VisibilityProgress
        {
            get => _visibilityProgress;
            set
            {
                _visibilityProgress = value;
                OnPropertyChanged(nameof(VisibilityProgress));
            }
        }
        public Visibility VisibilityProcess
        {
            get => _visibilityProcess;
            set
            {
                _visibilityProcess = value;
                OnPropertyChanged(nameof(VisibilityProcess));
            }
        }
        public RelayCommand VisibilityHide => new(VisibilityHideCommand, true);
        private void VisibilityHideCommand()
        {
            VisibilityDefaultAsShowed = Visibility.Visible;
            VisibilityDefaultAsHidden = Visibility.Hidden;
            VisibilityProgress = Visibility.Visible;
            VisibilityProcess = Visibility.Visible;
        }
        private void VisibilityShowCommand()
        {
            VisibilityDefaultAsShowed = Visibility.Hidden;
            VisibilityDefaultAsHidden = Visibility.Visible;
            VisibilityProgress = Visibility.Hidden;
            VisibilityProcess = Visibility.Hidden;
        }
        public RelayCommand ClearFile => new(ClearFileCommand, true);
        private void ClearFileCommand()
        {
            VisibilityHideCommand();
            _crypting.ClearFile();
            SFile = _crypting.UpdateSimpleFile();
            UpdatePageContent();
            FileDialogName = default;
            MoveDetails = false;
            MoveBars = false;
        }
        private void UpdatePageContent()
        {
            SFile = _crypting.UpdateSimpleFile();

            if (SFile.Status)
            {
                FileStatusText = "Encrypted";
                OpenPadlockVisibility = Visibility.Hidden;
                ClosePadlockVisibility = Visibility.Visible;
                CryptingVisibility = Visibility.Hidden;
                DecryptingVisibility = Visibility.Visible;
            }
            else
            {
                FileStatusText = "Decypted";
                OpenPadlockVisibility = Visibility.Visible;
                ClosePadlockVisibility = Visibility.Hidden;
                CryptingVisibility = Visibility.Visible;
                DecryptingVisibility = Visibility.Hidden;
            }


            SelectedCryptingMethod = _crypting.GetCryptingMethodName();
            FileContentText = _crypting.GetDataFromFile();
            FileNameText = SFile.Name;
            ProcessText = "";
            SizeText = SFile.SizeString;
            ExtensionText = SFile.Extension;
            MethodUsedText = SFile.Method;
        }

        private string _fileContentText;
        public string FileContentText
        {
            get => _fileContentText;
            set
            {
                _fileContentText = value;
                OnPropertyChanged(nameof(FileContentText));
            }
        }
        private string _methodUsedText;
        public string MethodUsedText
        {
            get => _methodUsedText;
            set
            {
                _methodUsedText = value;
                OnPropertyChanged(nameof(MethodUsedText));
            }
        }
        private string _extensionText;
        public string ExtensionText
        {
            get => _extensionText;
            set
            {
                _extensionText = value;
                OnPropertyChanged(nameof(ExtensionText));
            }
        }

        private string _sizeText;
        public string SizeText
        {
            get => _sizeText;
            set
            {
                _sizeText = value;
                OnPropertyChanged(nameof(SizeText));
            }
        }

        private string _fileStatusText;
        public string FileStatusText
        {
            get => _fileStatusText;
            set
            {
                _fileStatusText = value;
                OnPropertyChanged(nameof(FileStatusText));
            }
        }
        private string _cipherContentText;
        public string CipherContentText
        {
            get => _cipherContentText;
            set
            {
                _cipherContentText = value;
                OnPropertyChanged(nameof(CipherContentText));
            }
        }
        private string _cipherToolTip;
        public string CipherToolTip
        {
            get => _cipherToolTip;
            set
            {
                _cipherToolTip = value;
                OnPropertyChanged(nameof(CipherToolTip));
            }
        }

        private string _fileNameText;
        public string FileNameText
        {
            get => _fileNameText;
            set
            {
                _fileNameText = value;
                OnPropertyChanged(nameof(FileNameText));
            }
        }

        private string _fileDialogName;

        public string FileDialogName
        {
            get => _fileDialogName;
            set
            {
                _fileDialogName = value;
                OnPropertyChanged(nameof(FileDialogName));
            }
        }

        private BlurEffect _effect;
        private BlurEffect _effectCombo;

        public BlurEffect Effect
        {
            get => _effect;
            set
            {
                _effect = value;
                OnPropertyChanged(nameof(Effect));
            }
        }
        public BlurEffect EffectCombo
        {
            get => _effectCombo;
            set
            {
                _effectCombo = value;
                OnPropertyChanged(nameof(EffectCombo));
            }
        }
        public RelayCommand EffectComboFocusLost => new(EffectComboFocusLostCommand, true);

        private void EffectComboFocusLostCommand()
        {
            Effect = null;
            EffectCombo = null;
        }

        public RelayCommand EffectComboClick => new(EffectComboClickCommand, true);

        private void EffectComboClickCommand()
        {
            BlurEffect newEffect = new()
            {
                Radius = 15
            };
            Effect = newEffect;
            EffectCombo = null;
        }

        private bool _cryptingButtonEnabled = true;
        public bool CryptingButtonEnabled
        {
            get => _cryptingButtonEnabled;
            set
            {
                _cryptingButtonEnabled = value;
                OnPropertyChanged(nameof(CryptingButtonEnabled));
            }
        }

        public RelayCommand CopyRsaKeysToClipboard => new(CopyRsaKeysToClipboardCommand, true);

        private void CopyRsaKeysToClipboardCommand() => _crypting.GetClipboardString();

        public static double ExtendBarRange => BarExtension4KReady;

        private bool _moveBars = false;

        public bool MoveBars
        {
            get => _moveBars;
            set
            {
                _moveBars = value;
                OnPropertyChanged(nameof(MoveBars));
            }
        }

        private Visibility _rsaGridVisibility = Visibility.Hidden;
        private Visibility _cesarGridVisibility = Visibility.Hidden;
        private Visibility _cryptingVisibility = Visibility.Hidden;
        private Visibility _decryptingVisibility = Visibility.Hidden;
        public Visibility CryptingVisibility { get => _cryptingVisibility; set { _cryptingVisibility = value; OnPropertyChanged(nameof(CryptingVisibility)); } }
        public Visibility DecryptingVisibility { get => _decryptingVisibility; set { _decryptingVisibility = value; OnPropertyChanged(nameof(DecryptingVisibility)); } }
        public Visibility RsaGridVisibility { get => _rsaGridVisibility; set { _rsaGridVisibility = value; OnPropertyChanged(nameof(RsaGridVisibility)); } }
        public Visibility CesarGridVisibility { get => _cesarGridVisibility; set { _cesarGridVisibility = value; OnPropertyChanged(nameof(CesarGridVisibility)); } }

        public RelayCommand DisplayHelpMessage => new(DisplayHelpMessageCommand, true);

        private void DisplayHelpMessageCommand()
        {
            BlurWindow();
            string keyMessage = "helpText_cryptingPage";
            string keyTitle = "helpTitle_cryptingPage";
            string helpMessageString = (Application.Current as App).Resources.MergedDictionaries[0][keyMessage] as string;
            string helpTitleString = (Application.Current as App).Resources.MergedDictionaries[0][keyTitle] as string;
            DialogViewModel viewModel = new(helpMessageString, helpTitleString);
            _ = _dialogService.ShowDialog(viewModel);
            UnblurWindow();
        }

        private void BlurWindow()
        {
            BlurEffect newEffect = new()
            {
                Radius = 15
            };
            Effect = newEffect;
        }
        private void UnblurWindow()
        {
            Effect = null;
        }
    }
}
