using CrytonCore.Infra;
using CrytonCore.Model;
using System.Collections.ObjectModel;
using System.Windows.Media.Effects;

namespace CrytonCore.ViewModel
{
    public class SettingsViewModel : NotificationClass
    {
        private readonly App app = System.Windows.Application.Current as App;
        private readonly Theme theme = new();
        private readonly Language language = new();
        private readonly ObservableCollection<string> _themesCollection;
        private readonly ObservableCollection<string> _languagesCollection;
        private string _selectedTheme;
        private string _selectedLanguage;
        private BlurEffect effect;
        private BlurEffect effectCombo;

        public SettingsViewModel()
        {
            _languagesCollection = new ObservableCollection<string>(language.Get());
            _themesCollection = new ObservableCollection<string>(theme.Get());

            language.FirstRunLanguage();

            _selectedTheme = theme.GetThemeName();
            _selectedLanguage = language.GetLanguageName();

            SetGlobalLanguageShortName();
            SetGlobalLanguageAndTheme();
        }

        public ObservableCollection<string> ThemesCollection { get => _themesCollection; }
        public ObservableCollection<string> LanguagesCollection { get => _languagesCollection; }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                theme.SetTheme(_selectedTheme);
                SetGlobalLanguageAndTheme();
                OnPropertyChanged(nameof(SelectedTheme));
            }
        }
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                language.SetLanguage(_selectedLanguage);
                SetGlobalLanguageAndTheme();
                SetGlobalLanguageShortName();
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        private void SetGlobalLanguageAndTheme()
        {
            app.Resources.MergedDictionaries.Clear();
            app.Resources.MergedDictionaries.Add(language.GetDictionary());
            app.Resources.MergedDictionaries.Add(theme.GetDictionary());
        }
        private void SetGlobalLanguageShortName() => app.ShortLanguageName = language.GetLanguageShortString();

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

        private void EffectComboFocusLostCommand() => UnblurWindow();
        public RelayCommand EffectComboClick { get => new(EffectComboClickCommand, true); }

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
