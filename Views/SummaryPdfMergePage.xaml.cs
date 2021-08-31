using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for NewPdfMergeUserControl.xaml
    /// </summary>
    public partial class SummaryPdfMergePage : Page
    {
        public SummaryPdfMergePage()
        {
            InitializeComponent();
            (App.Current as App).themesUpdaters.Add(new App.UpdateThemes(UpdateTheme));
        }
        private void UpdateTheme()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml") });
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Save pdf file",
                Filter = "Pdf file (.pdf)|*.pdf" // Filter files by extension
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // try
                // {
                //     List<Tools.Image> outputImages = new List<Tools.Image>();
                //     foreach (var item in _foreignSimpleImages)
                //         outputImages.Add((App.Current as App).mapperService.MapperSimplePdf.Map<Tools.SimpleImageManager, Tools.Image>(item));
                //     _pdf.MergePdf(outputImages, saveFileDialog.FileName);
                //     saveButton.Command = DialogHost.OpenDialogCommand;
                // }
                // catch (Exception ex)
                // {
                //     var what = ex.Message;
                //     throw;
                // }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.GoPdfMergePage.Invoke();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
