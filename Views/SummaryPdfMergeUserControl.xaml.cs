using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for NewPdfMergeUserControl.xaml
    /// </summary>
    public partial class SummaryPdfMergeUserControl : UserControl
    {
        private System.Windows.Navigation.NavigationService _navigationService;
        public SummaryPdfMergeUserControl()
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
            SaveFileDialog saveFileDialog = new SaveFileDialog
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
                _navigationService = System.Windows.Navigation.NavigationService.GetNavigationService(this);
                var view = (Application.Current as App)?.pdfMergeUserControl;
                if (view != null)
                    _navigationService?.Navigate(view);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
