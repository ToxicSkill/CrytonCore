using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using static CrytonCore.Enums.EThemes;

namespace CrytonCore.Model
{
    public class Theme
    {
        public string ThemeName { get; set; }
        public int ThemeIndex { get; set; }
        public ObservableCollection<string> ThemesCollection { get; set; }
        private List<ResourceDictionary> ThemesDictionaries { get; set; }
        private List<ResourceDictionary> AccentThemesDictionaries { get; set; }
        public Theme()
        {
            InitializeComponents();
            SetTheme((int)Themes.Standard);
        }

        private void InitializeComponents()
        {
            ThemesCollection = new ObservableCollection<string> {
                EnumToString(Themes.Standard),
                EnumToString(Themes.Dark),
                EnumToString(Themes.Light)
            };

            AccentThemesDictionaries = new List<ResourceDictionary>(){
            new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Orange.xaml") },
            new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Orange.xaml") },
            new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Orange.xaml") },
            new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml") }
            };

            ThemesDictionaries = new List<ResourceDictionary>(){
            new ResourceDictionary() { Source = new Uri("..\\Dictionaries\\StandardThemeDictionary.xaml", UriKind.Relative) },
            new ResourceDictionary() { Source = new Uri("..\\Dictionaries\\DarkThemeDictionary.xaml", UriKind.Relative) },
            new ResourceDictionary() { Source = new Uri("..\\Dictionaries\\LightThemeDictionary.xaml", UriKind.Relative) }
            };
        }

        internal ObservableCollection<string> Get()
        {
            return ThemesCollection;
        }

        internal ResourceDictionary GetDictionary()
        {
            return ThemesDictionaries[ThemeIndex];
        }

        internal ResourceDictionary GetAccentDictionary()
        {
            return AccentThemesDictionaries[ThemeIndex];
        }


        internal void SetTheme(string input)
        {
            ThemeName = input;
            SetThemeIndexByName(ThemeName);
        }
        internal void SetTheme(int input)
        {
            ThemeIndex = input;
            SetThemeNameByIndex(input);
        }

        internal string GetThemeName()
        {
            return ThemeName;
        }
        internal int GetThemeIndex()
        {
            return ThemeIndex;
        }

        private void SetThemeIndexByName(string name)
        {
            ThemeIndex = ThemesCollection.IndexOf(name);
        }
        private void SetThemeNameByIndex(int index)
        {
            ThemeName = ThemesCollection[index];
        }
    }
}
