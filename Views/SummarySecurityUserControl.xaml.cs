using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CrytonCore.Files;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf.Security;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for UserControlNewSecurity.xaml
    /// </summary>
    public partial class NewSecurityUserControl : UserControl
    {
        private PdfFile _pdfFile = new();
        private PdfSharpCore.Pdf.PdfDocument _document = new();
        public NewSecurityUserControl()
        {
            InitializeComponent();
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

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((App.Current as App).AppCounter == 0)
            {
                (App.Current as App).AppCounter++;
                _pdfFile = (App.Current as App).PdfFile;
                labelFileName.Content = _pdfFile.Name;
                encryptionModeLabelContent.Content = _pdfFile.HighestEncryptionLevel ? "128 bit" : "40 bit";
                ownerPasswordLabel.Visibility = Visibility.Hidden;
                PdfEncryption();
            }
        }
        private void PdfEncryption()
        {
            if (_pdfFile.Name is null)
                return;
            // Open an existing document. Providing an unrequired password is ignored.
            _document = PdfReader.Open(_pdfFile.Url,"homik");

            PdfSecuritySettings securitySettings = _document.SecuritySettings;

            // Setting one of the passwords automatically sets the security level to 
            // PdfDocumentSecurityLevel.Encrypted128Bit.

            string ownerPassword = default;
            string userPassword = default;

            if (_pdfFile.RandomPassword == true)
            {
                userPassword = GenerateToken(9).PadLeft(8);
                ownerPassword = GenerateToken(9).PadLeft(8);

                if(_pdfFile.UserOwnerPassword == true)
                {
                    ownerPasswordLabel.Visibility = Visibility.Visible;
                    OwnerPasswordLabelContent.Visibility = Visibility.Visible;
                }
                else
                {
                    ownerPasswordLabel.Visibility = Visibility.Hidden;
                    OwnerPasswordLabelContent.Visibility = Visibility.Hidden;
                }

            }
            else
            {
                userPassword = _pdfFile.UserPassword;
                if (_pdfFile.UserOwnerPassword)
                {
                    ownerPassword = _pdfFile.OwnerPassword;
                    securitySettings.OwnerPassword = ownerPassword;
                    ownerPasswordLabel.Visibility = Visibility.Visible;
                    OwnerPasswordLabelContent.Visibility = Visibility.Visible;
                }
                else
                {
                    securitySettings.OwnerPassword = "";
                    ownerPasswordLabel.Visibility = Visibility.Hidden;
                    OwnerPasswordLabelContent.Visibility = Visibility.Hidden;
                }
            }

            UserPasswordLabelContent.Content = userPassword;
            OwnerPasswordLabelContent.Content = ownerPassword;

            securitySettings.UserPassword = userPassword;
            
            if (_pdfFile.HighestEncryptionLevel == true)
                securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted128Bit;
            else
                securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted40Bit;

            // Don't use 40 bit encryption unless needed for compatibility reasons
            //securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted40Bit;

            // Restrict some rights.
            securitySettings.PermitAccessibilityExtractContent = false;
            securitySettings.PermitAnnotations = false;
            securitySettings.PermitAssembleDocument = false;
            securitySettings.PermitExtractContent = false;
            securitySettings.PermitFormsFill = true;
            securitySettings.PermitFullQualityPrint = false;
            securitySettings.PermitModifyDocument = true;
            securitySettings.PermitPrint = false;

            // Save the document...
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Save pdf file",
                Filter = "Pdf file (.pdf)|*.pdf" // Filter files by extension
            };

            if (saveFileDialog.ShowDialog() == true)
                try
                {
                    _document.Save(saveFileDialog.FileName);
                    saveButton.Command = DialogHost.OpenDialogCommand;
                }
                catch (Exception ex)
                {
                    var what = ex.Message;
                    throw;
                }
        }

        private string GenerateToken(int length)
        {
            using (RNGCryptoServiceProvider cryptRNG = new())
            {
                byte[] tokenBuffer = new byte[length];
                cryptRNG.GetBytes(tokenBuffer);
                return Convert.ToBase64String(tokenBuffer);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => (Application.Current as App).AppCounter = 0;
    }
}
