using CrytonCore.Enums;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Numerics;
using System.Linq;
using static CrytonCore.Enums.EKeysFileIdentifiers;
using static CrytonCore.Enums.ETypesOfCrypting;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for InputCryptingWindow.xaml
    /// </summary>
    public partial class InputCryptingWindow : Window
    {
        private string[] _droppedString;
        private bool status = false;
        public string CurrentCipher { get; set; }
        public bool Status { get => status; set => Status = value; }
        public InputCryptingWindow()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (status)
                Close();
        }

        private void ApplyButton_MouseEnter(object sender, MouseEventArgs e) => applyButton.Opacity = 0.6;

        private void ApplyButton_MouseLeave(object sender, MouseEventArgs e) => applyButton.Opacity = 1;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        private void NewDrop(object sender, DragEventArgs e)
        {
            if (null != e.Data && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                _droppedString = e.Data.GetData(DataFormats.FileDrop) as string[];
            }
            _ = CheckFile("RSA");
        }

        private void DragDropButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() == true)
            {
                List<string> list = new()
                {
                        openFileDialog.FileName
                    };
                _droppedString = list.ToArray();
            }
            _ = CheckFile("RSA");
        }

        private bool CheckFile(string cipher)
        {
            long length = new FileInfo(_droppedString[0]).Length;
            if (length > (int)Math.Pow(2, 15))
                return false;
            switch (cipher)
            {
                case nameof(TypesOfCrypting.RSA):
                    List<string> lines = new(System.IO.File.ReadAllLines(_droppedString[0]));
                    if (lines.Count > 0)
                    {
                        if (lines[(int)KeysFileIdentifiers.PublicTitle].Equals("Public Keys: ") &&
                            lines[(int)KeysFileIdentifiers.PrivateTitle].Equals("Private Keys: ") &&
                            lines.Count == 6)
                        {
                            List<BigInteger> keys = new();
                            try
                            {
                                List<bool> isNumberHex = new();
                                List<int> fileLineIndex = new() {
                                    (int)KeysFileIdentifiers.PublicKeyFirst,
                                    (int)KeysFileIdentifiers.PublicKeySecond,
                                    (int)KeysFileIdentifiers.PrivateKeyFirst,
                                    (int)KeysFileIdentifiers.PrivateKeySecond,
                                  };
                                foreach (var id in fileLineIndex)
                                {
                                    if (OnlyHexInString(lines[id]))
                                    {
                                        isNumberHex.Add(true);
                                    }
                                    else if (OnlyDecInString(lines[id]))
                                    {
                                        isNumberHex.Add(false);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                foreach ((int index, int value) id in fileLineIndex.Select((value, index) => (index, value)))
                                {
                                    keys.Add(BigInteger.Parse(lines[id.value], isNumberHex[id.index] ?
                                        System.Globalization.NumberStyles.HexNumber :
                                        System.Globalization.NumberStyles.Number));
                                    if (keys[id.index] <= 0)
                                    {
                                        status = false;
                                        break;
                                    }
                                }
                                if (keys.Count == fileLineIndex.Count)
                                {
                                    status = true;
                                    privateKey.Text = keys[0].ToString();
                                    publicKey.Text = keys[1].ToString();
                                    pqKey.Text = keys[2].ToString();
                                    phiKey.Text = keys[3].ToString();
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            status = true;
                            (Application.Current as App).AppKeys.numericKeys = new List<BigInteger>(keys);
                        }
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
        // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
        private bool OnlyHexInString(string test) => System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        private bool OnlyDecInString(string test) => test.All(char.IsDigit);
    }
}
