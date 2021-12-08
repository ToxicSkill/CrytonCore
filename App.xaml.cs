using CrytonCore.Files;
using System;
using System.Collections.Generic;
using System.Windows;
using CrytonCore.Mapper;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Numerics;
using CrytonCore.Interfaces;
using CrytonCore.Helpers;
using CrytonCore.Views;
using CrytonCore.Model;

namespace CrytonCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Pages navigation

        public delegate void WelcomePageDelegate();
        public static WelcomePageDelegate GoStartPage;

        public delegate void CryptingPageDelegate();
        public static CryptingPageDelegate GoCryptingPage;

        public delegate void PdfManagerPageDelegate();
        public static PdfManagerPageDelegate GoPdfManagerPage;

        public delegate void SettingsPageDelegate();
        public static SettingsPageDelegate GoSettingsPage;

        public delegate void PdfMergePageDelegate();
        public static PdfMergePageDelegate GoPdfMergePage;

        public delegate void PdfSecurityPageDelegate();
        public static PdfSecurityPageDelegate GoPdfSecurityPage;

        public delegate void ImageToPdfPageDelegate();
        public static ImageToPdfPageDelegate GoImageToPdfPage;

        public delegate void PdfToImagePageDelegate();
        public static PdfToImagePageDelegate GoPdfToImagePage;

        public delegate void SummaryPdfMergePageDelegate(SummaryPdfMergePage summaryPdfMergePage);
        public static SummaryPdfMergePageDelegate GoSummaryPdfMergePage;

        public delegate void PasswordManagerPageDelegate();
        public static PasswordManagerPageDelegate GoPasswordManagerPage;

        #endregion  

        //public SummaryPdfMergeUserControl summaryPdfMergeUserControl;

        public MapperService mapperService = new();
        private readonly string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public bool PassingDroppedImagesFlag { get; set; } = false;

        private List<IHelpersInterface> CrytpingSettings { get; set; } = new List<IHelpersInterface>();

        public PdfFile PdfFile { get; set; } = new PdfFile();

        public List<System.Drawing.Image> ImagesApp { get; set; }

        public int AppCounter { get; set; } = 0;

        public List<string> droppedStringList = new();

        private string _currentErrorMessage;

        public readonly List<UpdateThemes> themesUpdaters = new();

        public delegate void UpdateThemes();

        public string ShortLanguageName { get; set; }

        public static bool AppIsLoaded { get; set; }
        public Keys AppKeys { get; set; } = new();


        public string GetCurrentErrorMessage
        {
            get => _currentErrorMessage.Length != 0 ? _currentErrorMessage : "No errors";
            set
            {
                if (value.Length != 0)
                    _currentErrorMessage = value;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        public static string GetDictionaryString(string name)
        {
            return App.Current.Resources.MergedDictionaries[0][name].ToString();
        }

        public Task<bool> SaveDataInRelativePath(string appendString, string[] data)
        {
            Func<bool> function = () =>
            {
                var newPath = appPath.Substring(0, appPath.Length - "Cryton.exe".Length);
                var fullPath = newPath + appendString;
                var directoryPath = Path.GetDirectoryName(fullPath);
                try
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        _ = Directory.CreateDirectory(directoryPath ?? string.Empty);
                    }
                    using (var writer = new StreamWriter(fullPath))
                    {
                        Task.WaitAll(data.Select(line => writer.WriteLineAsync(line)).ToArray());
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                return true;
            };
            return Task.Run(function);
        }
    }
}