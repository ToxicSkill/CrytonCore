using CrytonCore.Files;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for SecurityPage.xaml
    /// </summary>
    public partial class SecurityPage : Page
    {
        public Dictionary<ToggleButton, StackPanel> ToggleButtonsKeysToStackPanelOpacity { get; set; } = new Dictionary<ToggleButton, StackPanel>();

        private readonly PdfFile _pdfFile = new();
        public SecurityPage()
        {
            InitializeComponent();
            InitializeEntities();
            (App.Current as App).themesUpdaters.Add(new App.UpdateThemes(UpdateTheme));
        }
        private void UpdateTheme()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml") });

            //switch ((App.Current as App).GetThemeDictionary())
            //{
            //    case 1:
            //        this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Orange.xaml") });
            //        break;
            //    //case 2:
            //    //    this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml") });
            //    //    break;
            //    default:
            //        this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml") });
            //        break;
            //}
        }

        private void InitializeEntities()
        {
            AssignToggleButtonsStates();

            ToggleButtonsKeysToStackPanelOpacity.Add(randomTB, randomSP);
            ToggleButtonsKeysToStackPanelOpacity.Add(encryptionLevelTB, encryptionLevelSP);
            ToggleButtonsKeysToStackPanelOpacity.Add(userOwnerTB, userOwnerSP);
        }

        private void AssignToggleButtonsStates()
        {
            _pdfFile.HighestEncryptionLevel = (bool)this.encryptionLevelTB.IsChecked;
            _pdfFile.UserOwnerPassword = (bool)this.userOwnerTB.IsChecked;
            _pdfFile.RandomPassword = (bool)this.randomTB.IsChecked;
        }

        private void LoadFileButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var splitFileName = openFileDialog.FileName.Split(new char[] { '\\' });
                _pdfFile.Url = openFileDialog.FileName;
                _pdfFile.Name = splitFileName[splitFileName.Length - 1]; 
                AssignToggleButtonsStates();
                this.fileNameLabel.Content = _pdfFile.Name;
                this.loadFileButton.Visibility = Visibility.Hidden;
                this.conitnueButton.Visibility = Visibility.Visible;
                this.clearButton.Visibility = Visibility.Visible;
                this.loadFileButton.IsEnabled = false;
                this.conitnueButton.IsEnabled = true;
            }

            //Storyboard topLblExtend = this.FindResource("topLabelExtend") as Storyboard;
            //Storyboard.SetTarget(topLblExtend, this.topLabel);
            //topLblExtend.Begin();
        }

        private void ClearFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pdfFile.IsEmpty() == true  && loadFileButton.Visibility == Visibility.Hidden)
            {
                _pdfFile.Clear();
                this.fileNameLabel.Content = "";
                this.loadFileButton.Visibility = Visibility.Visible;
                this.conitnueButton.Visibility = Visibility.Hidden;
                this.clearButton.Visibility = Visibility.Hidden;
                this.loadFileButton.IsEnabled = true;
                this.conitnueButton.IsEnabled = false;
            }
        }
        private void ConitnueButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pdfFile.RandomPassword == false)
            {
                if (_pdfFile.UserPassword is null)
                {
                    conitnueButton.Command = null;
                    return;
                }
                else if (_pdfFile.UserPassword.Length == 0)
                {
                    conitnueButton.Command = null;
                    return;
                }
                if (_pdfFile.UserOwnerPassword)
                {
                    if (_pdfFile.OwnerPassword is null)
                    {
                        conitnueButton.Command = null;
                        return;
                    }
                    else if (_pdfFile.OwnerPassword.Length == 0)
                    {
                        conitnueButton.Command = null;
                        return;
                    }
                }
            }
            (Application.Current as App).PdfFile = _pdfFile;
            conitnueButton.Command = MaterialDesignThemes.Wpf.Transitions.Transitioner.MoveNextCommand;
        }

        private void EncryptionLevelToggleButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            _pdfFile.HighestEncryptionLevel = ChangeOpacity(sender as ToggleButton, (bool)(sender as ToggleButton).IsChecked);
        }

        private void RandomPasswordToggleButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            _pdfFile.RandomPassword =
                ChangeOpacity(sender as ToggleButton, (bool)(sender as ToggleButton).IsChecked);

            ChangeVisibility();
        }

        private void UserOwnerToggleButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            _pdfFile.UserOwnerPassword =
                ChangeOpacity(sender as ToggleButton, (bool)(sender as ToggleButton).IsChecked);

            ChangeVisibility();
        }
        private void ChangeVisibility()
        {
            passwordsGrid.Visibility = _pdfFile.RandomPassword ?
                Visibility.Hidden : Visibility.Visible;
            ownerPasswordGrid.Visibility = _pdfFile.UserOwnerPassword ?
                Visibility.Visible : Visibility.Hidden;
        }


        private bool ChangeOpacity(object sender, bool isChecked)
        {
            ToggleButtonsKeysToStackPanelOpacity[sender as ToggleButton].Opacity = isChecked ? 1 : 0.5;
            return  isChecked;
        }

        private void UserPassword_TextChanged(object sender, TextChangedEventArgs e) => _pdfFile.UserPassword = this.userPassword.Text.ToString();

        private void OwnerPassword_TextChanged(object sender, TextChangedEventArgs e) => _pdfFile.OwnerPassword = this.ownerPassword.Text.ToString();
    }
}

