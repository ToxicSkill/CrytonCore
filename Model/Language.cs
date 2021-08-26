using CrytonCore.Infra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace CrytonCore.Model
{
    public class Language 
    {
        public string LanguageName { get; set; }
        public int LanguageIndex { get; set; }

        public ObservableCollection<string> LanguagesCollection { get; set; }
        public List<ResourceDictionary> LanguagesDictionaries { get; set; }

        private List<string> LanguagesShort { get; set; } 

        public Language()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            LanguagesDictionaries = new List<ResourceDictionary>(){
                new ResourceDictionary() { Source = new Uri("..\\Resources\\StringResources.xaml", UriKind.Relative) },
                new ResourceDictionary() { Source = new Uri("..\\Resources\\StringResources.pl-PL.xaml", UriKind.Relative) },
                new ResourceDictionary() { Source = new Uri("..\\Resources\\StringResources.fr-FE.xaml", UriKind.Relative) },
                new ResourceDictionary() { Source = new Uri("..\\Resources\\StringResources.de-DE.xaml", UriKind.Relative) }
            };

            LanguagesShort = new List<string>()
            {
                "en-US",
                "pl-PL",
                "fr-FE",
                "de-DE"
            };

            LanguagesCollection = new ObservableCollection<string> {
                Enums.Enumerates.EnumToString(Enums.Enumerates.Languages.English),
                Enums.Enumerates.EnumToString(Enums.Enumerates.Languages.Polski),
                Enums.Enumerates.EnumToString(Enums.Enumerates.Languages.French),
                Enums.Enumerates.EnumToString(Enums.Enumerates.Languages.Deutsh)
            };
        }

        public void FirstRunLanguage()
        {
            string currentLanguageShortcut = Thread.CurrentThread.CurrentCulture.ToString().Substring(0, 2).ToLower();
            SetLanguage(LanguagesCollection[LanguagesShort.FindIndex(x => x.Contains(currentLanguageShortcut))]);
        }
        public string GetLanguageShortString()
        {
            return LanguagesShort[LanguageIndex];
        }
        internal ObservableCollection<string> Get()
        {
            return LanguagesCollection;
        }

        internal ResourceDictionary GetDictionary()
        {
            return LanguagesDictionaries[LanguageIndex];
        }
        internal void SetLanguage(string input)
        {
            LanguageName = input;
            SetLanguageIndexByName(LanguageName);
        }
        internal void SetLanguage(int input)
        {
            LanguageIndex = input;
            SetLanguageNameByIndex(input);
        }

        internal string GetLanguageName()
        {
            return LanguageName;
        }
        internal int GetLanguageIndex()
        {
            return LanguageIndex;
        }

        private void SetLanguageIndexByName(string name)
        {
            LanguageIndex = LanguagesCollection.IndexOf(name);
        }
        private void SetLanguageNameByIndex(int index)
        {
            LanguageName = LanguagesCollection[index];
        }
    }
}
