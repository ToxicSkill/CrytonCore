using CrytonCore.Design;
using CrytonCore.Infra;
using CrytonCore.Model;
using System.Collections.ObjectModel;
using System.Windows.Media.Effects;

namespace CrytonCore.ViewModel
{
    public class SettingsViewModel : NotificationClass
    {
        private readonly App _app = System.Windows.Application.Current as App;
        private readonly Theme _theme = new();
        private readonly Language _language = new();
        private string _selectedTheme;
        private string _selectedLanguage;
        private BlurEffect _effect;
        private BlurEffect _effectCombo;

        public SettingsViewModel()
        {
            LanguagesCollection = new ObservableCollection<string>(_language.Get());
            ThemesCollection = new ObservableCollection<string>(_theme.Get());

            _language.FirstRunLanguage();

            _selectedTheme = _theme.GetThemeName();
            _selectedLanguage = _language.GetLanguageName();

            SetGlobalLanguageShortName();
            SetGlobalLanguageAndTheme();
        }

        public ObservableCollection<string> ThemesCollection { get; }
        public ObservableCollection<string> LanguagesCollection { get; }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (value != _selectedTheme)
                {
                    _selectedTheme = value;
                    _theme.SetTheme(_selectedTheme);
                    SetGlobalLanguageAndTheme();
                    OnPropertyChanged(nameof(SelectedTheme));
                }
            }
        }
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (value != _selectedLanguage)
                {
                    _selectedLanguage = value;
                    _language.SetLanguage(_selectedLanguage);
                    SetGlobalLanguageAndTheme();
                    SetGlobalLanguageShortName();
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }
        }

        private void SetGlobalLanguageAndTheme()
        {
            _app.Resources.MergedDictionaries.Clear();
            _app.Resources.MergedDictionaries.Add(_language.GetDictionary());
            _app.Resources.MergedDictionaries.Add(_theme.GetDictionary());
        }
        private void SetGlobalLanguageShortName() => _app.ShortLanguageName = _language.GetLanguageShortString();

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

        private void EffectComboFocusLostCommand() => UnblurWindow();
        public RelayCommand EffectComboClick => new(EffectComboClickCommand, true);

        private void EffectComboClickCommand()
        {
            BlurWindow();
            UnblurCombo();
        }

        private void BlurWindow()
        {
            BlurEffect newEffect = new()
            {
                Radius = 15
            };
            Effect = newEffect;
        }
        private void UnblurWindow() => Effect = null;

        private void UnblurCombo() => EffectCombo = null;
    }
}
