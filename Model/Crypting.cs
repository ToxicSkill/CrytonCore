using CrytonCore.File;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrytonCore.Mapper;
using static CrytonCore.Enums.ETypesOfCrypting;

namespace CrytonCore.Model
{
    public class Crypting
    {
        private string CryptingMethodName { get; set; }
        private int CryptingMethodIndex { get; set; }
        private ObservableCollection<string> CryptingMethodsCollection { get; set; }

        private readonly MapperService _fileMapper = new();

        private List<Cipher> _ciphers = new();
        
        private TypesOfCrypting CurrentCipher { get; set; }

        private CrytonFile File { get; set; }

        public Crypting()
        {
            InitializeComponents();
            SetCryptingMethod((int)TypesOfCrypting.CESAR);
            File = new CrytonFile();
        }

        private void InitializeComponents()
        {
            CryptingMethodsCollection = new ObservableCollection<string> {
                EnumToString(TypesOfCrypting.CESAR),
                EnumToString(TypesOfCrypting.RSA),
                EnumToString(TypesOfCrypting.TRANSPOSABLE)
            };
            _ciphers = new List<Cipher>()
            {
                new Ciphers.Cesar(),
                new Ciphers.RSA(),
                new Ciphers.Transposable()
            };
        }

        internal ObservableCollection<string> Get()
        {
            return CryptingMethodsCollection;
        }
        internal void SetCryptingMethod(string input)
        {
            CryptingMethodName = input;
            SetCryptingMethodIndexByName(CryptingMethodName);
        }

        private void SetCryptingMethod(int input)
        {
            CryptingMethodIndex = input;
            SetCryptingMethodNameByIndex(input);
        }

        internal string GetCryptingMethodName()
        {
            return CryptingMethodName;
        }
        internal int GetCryptingMethodIndex()
        {
            return CryptingMethodIndex;
        }

        private void SetCryptingMethodIndexByName(string name)
        {
            CryptingMethodIndex = CryptingMethodsCollection.IndexOf(name);
        }
        private void SetCryptingMethodNameByIndex(int index)
        {
            CryptingMethodName = CryptingMethodsCollection[index];
        }
        internal SimpleFile UpdateSimpleFile() => _fileMapper.Mapper.Map<CrytonFile, SimpleFile>(File);
       
        internal string GetDataFromFile()
        {
            if (File.Size != 0)
            {
                //return System.Text.Encoding.Default.GetString(_file.Data.Take(1000).ToArray());
                if (File.DivData[0].Length > 1000)
                    return Encoding.Default.GetString(File.DivData[0].Take(1000).ToArray());
                else
                {
                    int numberOfChunksToDisplay = (int)Math.Ceiling((double)(1000 / File.DivData[0].Length));
                    StringBuilder dataToDisplay = new();
                    if (numberOfChunksToDisplay <= File.DivData.Count)
                    {
                        for (int i = 0; i < numberOfChunksToDisplay; i++)
                        {                 //   return System.Text.Encoding.Default.GetString(_file.DivData.SelectMany(x => x).ToArray());
                            _ = dataToDisplay.Append(Encoding.Default.GetString(File.DivData[i].ToArray()));
                        }
                        return dataToDisplay.ToString();
                    }
                    else
                        return Encoding.Default.GetString(File.DivData[0].ToArray());

                    //   return System.Text.Encoding.Default.GetString(_file.DivData.SelectMany(x => x).ToArray());
                }
            }
            else
                return null;
        }

        internal void GetClipboardString()
        {
            if (CurrentCipher == TypesOfCrypting.RSA)
                if (File.ClipboardString.Length != 0)
                    System.Windows.Clipboard.SetText(File.ClipboardString);

        }
        internal async Task<bool> Crypt(IProgress<int> progress, CancellationToken cancellation)
        {
            var cipherObject = _ciphers.FirstOrDefault(x => String.Equals(x.Name.ToString(), CryptingMethodName, StringComparison.CurrentCultureIgnoreCase));
            if (cipherObject != null)
            {
                var cipher = (Cipher)Activator.CreateInstance(cipherObject.GetType(), File);
                CurrentCipher = (TypesOfCrypting)_ciphers.IndexOf(cipher);
                if (cipher != null)
                {
                    cipher.Dispose();
                    return File.Status
                        ? await cipher.Decrypt(progress, cancellation)
                        : await cipher.Encrypt(progress, cancellation);
                }
            }
            return false;
        }

        internal async Task<bool> LoadFile(string fileName)
        {
            ClearFile();
            var result = await File.LoadFileAsync(fileName);
            if (result && File.Method != "N/A")
                SetCryptingMethod(File.Method);
            return result;
        }

        internal async Task<bool> SaveFile(IProgress<string> progressIndicator)
        {
            return await File.SaveFileAsync(progressIndicator);
        }

        internal void ClearFile()
        {
            File = new CrytonFile();
            File.Dispose();
        }
    }
}
