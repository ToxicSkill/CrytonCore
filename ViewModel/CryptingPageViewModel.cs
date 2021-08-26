using CrytonCore.File;
using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using CrytonCore.Model;
using CrytonCore.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace CrytonCore.ViewModel
{
    public class CryptingPageViewModel : NotificationClass, IFileDragDropTarget
    {
        public SimpleFile SFile { get; set; } = new SimpleFile();
        public bool IsRunning { get; set; } = false;

        private readonly Crypting Crypting = new();
        private Dictionary<string, object> GridsDict;
        private readonly Dictionary<Enums.Enumerates.TypesOfCrypting, IHelpersInterface> CrytpingSettings = new();

        private readonly CancellationTokenSource cts = new();
        private string selectedCryptingMethod;
        private const double barExtension4KReady = 3840;
        private bool moveDetails;
        private double opacity = 0.6;
        private double opacityHelp = 1;
        private int progressDivisor = 1;
        private static readonly int secondsDelay = 2;

        private readonly DispatcherTimer infoTime = new()
        {
            Interval = TimeSpan.FromSeconds(secondsDelay)
        };

        private readonly IDialogService dialogService;

        public CryptingPageViewModel()
        {
            dialogService = new DialogService();
            dialogService.Register<DialogViewModel, DialogWindow>();

            InitializeCryptingSettings();

            CryptingMethodsCollection = Crypting.Get();
            selectedCryptingMethod = Crypting.GetCryptingMethodName();

            moveDetails = false;

            infoTime.Tick += InfoTimer_Tick;

            InitializeGridsDictionary();
            SetActualGridVisibility();
        }

        private void InitializeCryptingSettings()
        {
            CrytpingSettings.Add(Enums.Enumerates.TypesOfCrypting.RSA, new RSAHelper());
            CrytpingSettings.Add(Enums.Enumerates.TypesOfCrypting.CESAR, new CesarHelper());

            RsaCollection = CrytpingSettings[Enums.Enumerates.TypesOfCrypting.RSA].GetFirst();
            SelectedRsaCollection = CrytpingSettings[Enums.Enumerates.TypesOfCrypting.RSA].GetSelectedItemFirst();

            CesarCollection = CrytpingSettings[Enums.Enumerates.TypesOfCrypting.CESAR].GetFirst();
            SelectedCesarCollection = CrytpingSettings[Enums.Enumerates.TypesOfCrypting.CESAR].GetSelectedItemFirst();
        }

        private void InitializeGridsDictionary()
        {
            GridsDict = new Dictionary<string, object>
            {
                { Enums.Enumerates.TypesOfCrypting.CESAR.ToString("g"), typeof(CryptingPageViewModel).GetProperty("CesarGridVisibility") },
                { Enums.Enumerates.TypesOfCrypting.RSA.ToString("g"), typeof(CryptingPageViewModel).GetProperty("RSAGridVisibility") }
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
                object property = GridsDict[Crypting.GetCryptingMethodName()];
                string[] propertyName = property.ToString().Split(' ');
                (property as PropertyInfo).SetValue(this, Visibility.Visible);
                OnPropertyChanged(propertyName[1]);
            }
            catch (Exception) { }
        }

        public async Task<bool> LoadFile(string fileName)
        {
            bool loadingFileResult = false;
            try
            {
                loadingFileResult = await Crypting.LoadFile(fileName).ConfigureAwait(false);
            }
            catch (Exception) { }
            if (loadingFileResult)
            {
                UpdatePageContent();
                VisibilityShowCommand();
                MoveDetails = true;
                MoveBars = true;
            }
            return loadingFileResult;
        }
        public RelayCommand SaveFile { get => new(SaveFileCommand, true); }

        private async void SaveFileCommand()
        {
            Progress<string> progressIndicator = new();
            progressIndicator.ProgressChanged += StringReportProgress;

            try
            {
                VisibilityProgress = Visibility.Visible;
                await Crypting.SaveFile(progressIndicator);
            }
            catch (Exception) { }
            finally
            {
                infoTime.Start();
            }
        }
        private void StringReportProgress(object sender, string e) => ProcessText = e;

        public async void OnFileDropAsync(string[] filePaths)
        {
            _ = await LoadFile(filePaths[0]);
        }

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
                bool result = await Crypting.Crypt(progressIndicator, cts.Token);
                SFile = Crypting.UpdateSimpleFile();
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
            if (Crypting.GetCryptingMethodName().ToUpper() == Enums.Enumerates.EnumToString(Enums.Enumerates.TypesOfCrypting.RSA).ToUpper())
            {
                BlurWindow();
                Views.InputCryptingWindow dlg = new();
                dlg.ShowDialog();
                UnblurWindow();
            }
        }

        private void BeforeCipherAction()
        {
            IsRunning = true;
            VisibilityProcess = Visibility.Visible;
            progressDivisor = SFile.ChunkSize;
            //  cryptingButtonEnabled = false;
            //OnPropertyChanged("CryptingButtonEnabled");
        }
        private void AfterCipherAction()
        {
            VisibilityProcess = Visibility.Hidden;
            ProgressText = "";
            IsRunning = false;
            progressDivisor = 1;
            //OnPropertyChanged("CryptingButtonEnabled");
        }
        public RelayCommand CancelAction => new(CancelActionCommand, true);

        private void CancelActionCommand()
        {
            cts.Cancel();
        }
        private void PercentageReportProgress(object sender, int value)
        {
            SetProgressText((value * 100 / progressDivisor) + "%");
        }
        private void InfoTimer_Tick(object sender, EventArgs e) => ClearProcessInfo();

        private void ClearProcessInfo()
        {
            infoTime.Stop();
            ProgressText = "";
            ProcessText = "";
            VisibilityProgress = Visibility.Hidden;
        }

        private string progressText;

        public string GetProgressText()
        {
            return progressText;
        }
        public string ProgressText
        {
            get => progressText;
            set
            {
                progressText = value;
                OnPropertyChanged(nameof(ProgressText));
            }
        }

        public void SetProgressText(string value)
        {
            ProgressText = value;
        }
        private string processText;
        public string ProcessText
        {
            get => processText;
            set
            {
                processText = value;
                OnPropertyChanged(nameof(ProcessText));
            }
        }
        public string SelectedCryptingMethod
        {
            get => selectedCryptingMethod;
            set
            {
                selectedCryptingMethod = value;
                Crypting.SetCryptingMethod(selectedCryptingMethod);
                SetActualGridVisibility();
                OnPropertyChanged(nameof(SelectedCryptingMethod));
            }
        }
        public ObservableCollection<string> CryptingMethodsCollection { get; }

        private string selectedRsaCollection;
        public string SelectedRsaCollection
        {
            get => CrytpingSettings[Enums.Enumerates.TypesOfCrypting.RSA].GetSelectedItemFirst();
            set
            {
                selectedRsaCollection = value;
                // crytpingSettings[(int)Enums.Enumerates.TypesOfCrypting.RSA].SetSelectedItemFirst( selectedRsaCollection);
                CrytpingSettings[Enums.Enumerates.TypesOfCrypting.RSA].SetSelectedItemFirst(selectedRsaCollection);
                OnPropertyChanged(nameof(SelectedRsaCollection));
            }
        }
        public ObservableCollection<string> RsaCollection { get; set; }

        private string selectedCesarCollection;
        public string SelectedCesarCollection
        {
            get => CrytpingSettings[Enums.Enumerates.TypesOfCrypting.CESAR].GetSelectedItemFirst();
            set
            {
                selectedCesarCollection = value;
                // crytpingSettings[(int)Enums.Enumerates.TypesOfCrypting.RSA].SetSelectedItemFirst( selectedRsaCollection);
                CrytpingSettings[Enums.Enumerates.TypesOfCrypting.CESAR].SetSelectedItemFirst(selectedCesarCollection);
                OnPropertyChanged(nameof(SelectedCesarCollection));
            }
        }
        public ObservableCollection<string> CesarCollection { get; set; }

        public bool MoveDetails
        {
            get => moveDetails;
            set
            {
                moveDetails = value;
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
            get => opacity;
            set
            {
                opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }

        public RelayCommand ChangeOpacityFull => new(OpacityChangeFull, true);
        private void OpacityChangeFull() => Opacity = 1;
        public RelayCommand ChangeOpacityPartial => new(OpacityChangePartial, true);
        private void OpacityChangePartial() => Opacity = 0.6;
        public double OpacityHelp
        {
            get => opacityHelp;
            set
            {
                opacityHelp = value;
                OnPropertyChanged(nameof(OpacityHelp));
            }
        }

        public RelayCommand ChangeHelpOpacityFull => new(OpacityHelpChangeFull, true);
        private void OpacityHelpChangeFull() => OpacityHelp = 1;
        public RelayCommand ChangeHelpOpacityPartial => new(OpacityHelpChangePartial, true);
        private void OpacityHelpChangePartial() => OpacityHelp = 0.6;

        private Visibility visibilityHidden = Visibility.Hidden;
        private Visibility visibilityShowed = Visibility.Visible;
        private Visibility visibilityProgress = Visibility.Hidden;
        private Visibility visibilityProcess = Visibility.Hidden;
        private Visibility openPadlockVisibility = Visibility.Hidden;
        private Visibility closePadlockVisibility = Visibility.Hidden;

        public Visibility OpenPadlockVisibility { get => openPadlockVisibility; set { openPadlockVisibility = value; OnPropertyChanged(nameof(OpenPadlockVisibility)); } }
        public Visibility ClosePadlockVisibility { get => closePadlockVisibility; set { closePadlockVisibility = value; OnPropertyChanged(nameof(ClosePadlockVisibility)); } }
        public Visibility VisibilityDefaultAsShowed
        {
            get => visibilityShowed;
            set
            {
                visibilityShowed = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsShowed));
            }
        }
        public Visibility VisibilityDefaultAsHidden
        {
            get => visibilityHidden;
            set
            {
                visibilityHidden = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsHidden));
            }
        }
        public Visibility VisibilityProgress
        {
            get => visibilityProgress;
            set
            {
                visibilityProgress = value;
                OnPropertyChanged(nameof(VisibilityProgress));
            }
        }
        public Visibility VisibilityProcess
        {
            get => visibilityProcess;
            set
            {
                visibilityProcess = value;
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
            Crypting.ClearFile();
            SFile = Crypting.UpdateSimpleFile();
            UpdatePageContent();
            FileDialogName = default;
            MoveDetails = false;
            MoveBars = false;
        }
        private void UpdatePageContent()
        {
            SFile = Crypting.UpdateSimpleFile();

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


            SelectedCryptingMethod = Crypting.GetCryptingMethodName();
            FileContentText = Crypting.GetDataFromFile();
            FileNameText = SFile.Name;
            ProcessText = "";
            SizeText = SFile.SizeString;
            ExtensionText = SFile.Extension;
            MethodUsedText = SFile.Method;
        }

        private string fileContentText;
        public string FileContentText
        {
            get => fileContentText;
            set
            {
                fileContentText = value;
                OnPropertyChanged(nameof(FileContentText));
            }
        }
        private string methodUsedText;
        public string MethodUsedText
        {
            get => methodUsedText;
            set
            {
                methodUsedText = value;
                OnPropertyChanged(nameof(MethodUsedText));
            }
        }
        private string extensionText;
        public string ExtensionText
        {
            get => extensionText;
            set
            {
                extensionText = value;
                OnPropertyChanged(nameof(ExtensionText));
            }
        }

        private string sizeText;
        public string SizeText
        {
            get => sizeText;
            set
            {
                sizeText = value;
                OnPropertyChanged(nameof(SizeText));
            }
        }

        private string fileStatusText;
        public string FileStatusText
        {
            get => fileStatusText;
            set
            {
                fileStatusText = value;
                OnPropertyChanged(nameof(FileStatusText));
            }
        }
        private string cipherContentText;
        public string CipherContentText
        {
            get => cipherContentText;
            set
            {
                cipherContentText = value;
                OnPropertyChanged(nameof(CipherContentText));
            }
        }
        private string cipherToolTip;
        public string CipherToolTip
        {
            get => cipherToolTip;
            set
            {
                cipherToolTip = value;
                OnPropertyChanged(nameof(CipherToolTip));
            }
        }

        private string fileNameText;
        public string FileNameText
        {
            get => fileNameText;
            set
            {
                fileNameText = value;
                OnPropertyChanged(nameof(FileNameText));
            }
        }

        private string fileDialogName;

        public string FileDialogName
        {
            get => fileDialogName;
            set
            {
                fileDialogName = value;
                OnPropertyChanged(nameof(FileDialogName));
            }
        }

        private BlurEffect effect;
        private BlurEffect effectCombo;

        public BlurEffect Effect
        {
            get => effect;
            set
            {
                effect = value;
                OnPropertyChanged(nameof(Effect));
            }
        }
        public BlurEffect EffectCombo
        {
            get => effectCombo;
            set
            {
                effectCombo = value;
                OnPropertyChanged(nameof(EffectCombo));
            }
        }
        public RelayCommand EffectComboFocusLost { get => new(EffectComboFocusLostCommand, true); }

        private void EffectComboFocusLostCommand()
        {
            Effect = null;
            EffectCombo = null;
        }

        public RelayCommand EffectComboClick { get => new(EffectComboClickCommand, true); }

        private void EffectComboClickCommand()
        {
            BlurEffect newEffect = new()
            {
                Radius = 15
            };
            Effect = newEffect;
            EffectCombo = null;
        }

        private bool cryptingButtonEnabled = true;
        public bool CryptingButtonEnabled
        {
            get => cryptingButtonEnabled;
            set
            {
                cryptingButtonEnabled = value;
                OnPropertyChanged(nameof(CryptingButtonEnabled));
            }
        }

        public RelayCommand CopyRsaKeysToClipboard { get => new(CopyRsaKeysToClipboardCommand, true); }

        private void CopyRsaKeysToClipboardCommand() => Crypting.GetClipboardString();

        public double ExtendBarRange => barExtension4KReady;

        private bool moveBars = false;

        public bool MoveBars
        {
            get => moveBars;
            set
            {
                moveBars = value;
                OnPropertyChanged(nameof(MoveBars));
            }
        }

        private Visibility rsaGridVisibility = Visibility.Hidden;
        private Visibility cesarGridVisibility = Visibility.Hidden;
        private Visibility cryptingVisibility = Visibility.Hidden;
        private Visibility decryptingVisibility = Visibility.Hidden;
        public Visibility CryptingVisibility { get => cryptingVisibility; set { cryptingVisibility = value; OnPropertyChanged(nameof(CryptingVisibility)); } }
        public Visibility DecryptingVisibility { get => decryptingVisibility; set { decryptingVisibility = value; OnPropertyChanged(nameof(DecryptingVisibility)); } }
        public Visibility RSAGridVisibility { get => rsaGridVisibility; set { rsaGridVisibility = value; OnPropertyChanged(nameof(RSAGridVisibility)); } }
        public Visibility CesarGridVisibility { get => cesarGridVisibility; set { cesarGridVisibility = value; OnPropertyChanged(nameof(CesarGridVisibility)); } }

        public RelayCommand DisplayHelpMessage { get => new(DisplayHelpMessageCommand, true); }

        private void DisplayHelpMessageCommand()
        {
            BlurWindow();
            string keyMessage = "helpText_cryptingPage";
            string keyTitle = "helpTitle_cryptingPage";
            string helpMessageString = (Application.Current as App).Resources.MergedDictionaries[0][keyMessage] as string;
            string helpTitleString = (Application.Current as App).Resources.MergedDictionaries[0][keyTitle] as string;
            DialogViewModel viewModel = new(helpMessageString, helpTitleString);
            dialogService.ShowDialog(viewModel);
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
