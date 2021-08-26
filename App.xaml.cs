using CrytonCore.Files;
using System;
using System.Collections.Generic;
using System.Windows;
using CrytonCore.Model;
using CrytonCore.Mapper;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Numerics;
using CrytonCore.Interfaces;
using CrytonCore.Helpers;
using CrytonCore.Views;
using System.Drawing;

namespace CrytonCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //PAGES - NAVIAGTION

        public CryptPage cryptWindow;
        public PdfManagerPage pdfManagerPage;
        public SettingsPage settingsPage;
        public WelcomePage welcomePage;
        public SecurityUserControl securityUserControl;
        public PdfMergeUserControl pdfMergeUserControl;
        public ImageToPdfUserControl imageToPdfUserControl;
        public PdfToImageUserControl pdfToImageUserControl;
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

        public List<SimpleImageManager> simpleImages = new();

        public readonly List<UpdateThemes> themesUpdaters = new();

        public delegate void UpdateThemes();

        public string ShortLanguageName { get; set; }

        public Keys AppKeys { get; set; } = new Keys();

        public string GetCurrentErrorMessage
        {
            get => _currentErrorMessage.Length != 0 ? _currentErrorMessage : "No errors";
            set
            {
                if (value.Length != 0)
                    _currentErrorMessage = value;
            }
        }

        public class Keys
        {
            public List<BigInteger> numericKeys = new();
            public List<string> stringKeys = new();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeCryptingSettings();
            InitializePages();
            base.OnStartup(e);
        }


        private void InitializeCryptingSettings()
        {
            CrytpingSettings.Add(new RSAHelper());
        }

        private void InitializePages()
        {
            securityUserControl = new SecurityUserControl();
            pdfMergeUserControl = new PdfMergeUserControl();
            imageToPdfUserControl = new ImageToPdfUserControl();
            //summaryPdfMergeUserControl = new SummaryPdfMergeUserControl();
            pdfToImageUserControl = new PdfToImageUserControl();
            cryptWindow = new CryptPage();
            pdfManagerPage = new PdfManagerPage();
            settingsPage = new SettingsPage();
            welcomePage = new WelcomePage();
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
                        Directory.CreateDirectory(directoryPath ?? string.Empty);
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