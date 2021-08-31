using CrytonCore.File;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrytonCore.Mapper;

namespace CrytonCore.Model
{
    public class Crypting
    {
        public string CryptingMethodName { get; set; }
        public int CryptingMethodIndex { get; set; }
        public ObservableCollection<string> CryptingMethodsCollection { get; set; }

        private readonly MapperService fileMapper = new();

        private List<Cipher> Ciphers = new();
        
        private Enums.Enumerates.TypesOfCrypting CurrentCipher { get; set; }

        public CrytonFile File { get; set; }

        public Crypting()
        {
            InitializeComponents();
            SetCryptingMethod((int)Enums.Enumerates.TypesOfCrypting.CESAR);
            File = new CrytonFile();
        }

        private void InitializeComponents()
        {
            CryptingMethodsCollection = new ObservableCollection<string> {
                Enums.Enumerates.EnumToString(Enums.Enumerates.TypesOfCrypting.CESAR),
                Enums.Enumerates.EnumToString(Enums.Enumerates.TypesOfCrypting.RSA),
                Enums.Enumerates.EnumToString(Enums.Enumerates.TypesOfCrypting.TRANSPOSABLE)
            };
            Ciphers = new List<Cipher>()
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
        internal void SetCryptingMethod(int input)
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
        internal SimpleFile UpdateSimpleFile() => fileMapper.Mapper.Map<CrytonFile, SimpleFile>(File);
       
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
                            dataToDisplay.Append(Encoding.Default.GetString(File.DivData[i].ToArray()));
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
            if (CurrentCipher == Enums.Enumerates.TypesOfCrypting.RSA)
                if (File.ClipboardString.Length != 0)
                    System.Windows.Clipboard.SetText(File.ClipboardString);

        }
        internal async Task<bool> Crypt(IProgress<int> progress, CancellationToken cancellation)
        {
            Cipher cipherObject = Ciphers.FirstOrDefault(x => x.Name.ToString().ToLower() == CryptingMethodName.ToLower());
            Cipher _cipher = (Cipher)Activator.CreateInstance(cipherObject.GetType(), File);
            CurrentCipher = (Enums.Enumerates.TypesOfCrypting)Ciphers.IndexOf(_cipher);
            _cipher.Dispose();
            return File.Status ? await _cipher.Decrypt(progress, cancellation) : await _cipher.Encrypt(progress, cancellation);
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
