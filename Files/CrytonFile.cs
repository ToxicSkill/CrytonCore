using System;
using static CrytonCore.Enums.Enumerates;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text;
using Microsoft.Win32;
using CrytonCore.Ciphers;

namespace CrytonCore
{
    public class AuditableModel
    {
        public int CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
    }

    public class CrytonFile : AuditableModel, IDisposable
    {
        private const int _recognizableBytesLength = 128;
        private const int _controlBytesLength = 32;
        private const int _contentBytesLength = 96;
        private bool _disposed = false;
        private byte[] RecognizableBytesFromEncryption = new byte[_recognizableBytesLength];
        private Random rand = new();

        // Instantiate a SafeHandle instance.
        private readonly SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        public string Url { get; set; }
        public string Name { get; set; }
        public string RequestedName { get; set; }
        public uint Size { get; set; }
        public string SizeString { get; set; }
        public string Extension { get; set; }
        public string RequestedExtension { get; set; }
        public string Method { get; set; }
        public int MethodId { get; set; }
        public int ChunkSize { get; set; }
        public string ExtraFileInfo { get; set; }
        public byte[] Data { get; set; }
        public List<byte[]> DivData { get; set; }
        public string ClipboardString { get; set; }

        /// <summary>
        /// True - encrypted False - decrypted
        /// </summary>
        public bool Status { get; set; }

        public bool Exist { get; set; }

        private readonly List<int> RecognizableLengths = new() { 
            12, // method
            10, //extension
            24, // extra file info
            46 //name (short)
        };

        public CrytonFile() { }

        public async Task<bool> LoadFileAsync(string path)
        {
            Size = (uint)new FileInfo(path).Length;

            if (Size.Equals(0))
                return Exist = false;

            Method = "N/A";

            bool skip = false;
            if(Size > _recognizableBytesLength)
            {
                if(CheckOriginOfFile(System.IO.File.ReadAllBytes(path).Take(_recognizableBytesLength).ToArray()))
                {
                    Size -= _recognizableBytesLength;
                    Status = true;
                    skip = true;
                }
            }

            SizeString = GetSizeString();

            int chunkSize = GetChunktSize();
            try
            {
                DivData = skip ? await Task.Run(() => new List<byte[]>(ArraySplit(FileToByteArray(path).Skip(_recognizableBytesLength).ToArray(), chunkSize))).ConfigureAwait(false) 
                    : await Task.Run(() => new List<byte[]>(ArraySplit(FileToByteArray(path), chunkSize))).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return false;
            }

            ChunkSize = DivData.Count;
            Url = path;
            GetFileNames(Url.Split(new char[] { '\\' }));

            //ExtraFileInfo = default;
            ExtraFileInfo = "";

            return Exist = DivData.Count != 0 && Size != 0;
        }
      
        public async Task<bool> SaveFileAsync(IProgress<string> progress)
        {
            if (Exist is false)
                return false;

            progress.Report("Preparing file to save...");


            if (Status)
            {
                if(PrepareRerecognizableBytes())
                    DivData.Insert(0, RecognizableBytesFromEncryption);
            }

            if(RequestedExtension == null || RequestedName == null)
            {
                RequestedName = Name;
                RequestedExtension = Extension;
            }

            SaveFileDialog saveFileDialog = new()
            {
                Title = Status ? "Save encrypted file" : "Save decrypted file",
                DefaultExt = ".dat", // Default file extension
                Filter = Status ? 
                "Text File (.txt)|*.txt | Data files (.dat)|*.dat" :
                "Original extension (." + RequestedExtension.ToLower().ToString() + ")|*." + RequestedExtension.ToLower().ToString() // Filter files by extension
            };

            try
            {
                if (saveFileDialog.ShowDialog() == true)
                    await Task.Run(() => ByteArrayToFile(saveFileDialog.FileName));
            }
            catch (Exception)
            {
                progress.Report("Error during saving file");
                return false;
                throw;
            }

            progress.Report("File has been successfuly saved");
            return true;
        }

        private void GetFileNames(string[] splitName)
        {
            if (splitName.Length >= 1)
            {
                Name = splitName[^1];
                string[] splitExtension = Name.Split(new char[] { '.' });

                if (splitExtension.Length >= 1)
                    Extension = splitExtension[^1];

                Name = Name.Remove(Name.Length - Extension.Length - 1);
            }
        }

        private string GetSizeString()
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = Size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                ++order;
                len /= 1024;
            }
            return string.Format("{0:0.## }{1}", len, sizes[order]);
        }

        public string GetMethodFromID(int ID)
        {
            var enumDisplayStatus = (TypesOfCrypting)ID;
            return Method = enumDisplayStatus.ToString();
        }

        public static byte[] FileToByteArray(string fileName) => System.IO.File.ReadAllBytes(fileName);

        public void ByteArrayToFile(string fileName) => System.IO.File.WriteAllBytes(fileName, DivData.SelectMany(x => x).ToArray());

        private static int GetChunktSize()
        {
            int halfMegabyte = (int)Math.Pow(2, 19);
            return halfMegabyte;// fileSize / halfMegabyte > 1 ? halfMegabyte : fileSize;
        }

        private IEnumerable<byte[]> ArraySplit(byte[] bArray, int intBufforLengt)
        {
            int bArrayLenght = bArray.Length;
            int i = 0;
            byte[] bReturn;

            for (; bArrayLenght > (i + 1) * intBufforLengt; i++)
            {
                bReturn = new byte[intBufforLengt];
                Array.Copy(bArray, i * intBufforLengt, bReturn, 0, intBufforLengt);
                yield return bReturn;
            }

            int intBufforLeft = bArrayLenght - i * intBufforLengt;
            if (intBufforLeft > 0)
            {
                bReturn = new byte[intBufforLeft];
                Array.Copy(bArray, i * intBufforLengt, bReturn, 0, intBufforLeft);
                yield return bReturn;
            }
        }

        private string GetRandomChar(int extraLength)
        {
            StringBuilder sb = new();
            for (int i = 0; i < extraLength; i++)
            {
                sb.Append((char)rand.Next(0, 255));
            }
            return sb.ToString();
        }

        public bool PrepareRerecognizableBytes()
        {
            byte[] recognizableArray = new byte[_recognizableBytesLength];
            byte[] contentArray = new byte[_contentBytesLength];
            int offset = 0;
            byte[] relativeLenthsArray = new byte[RecognizableLengths.Count];

            try
            {
                List<string> listOfContent = new()
                {
                    Method,
                    Extension,
                    ExtraFileInfo,
                    Name.Substring(0, RecognizableLengths[3] < Name.Length ? RecognizableLengths[3] : Name.Length)
                };

                foreach (var contentItem in listOfContent.Select((value, index) => new { value, index }))
                {
                    relativeLenthsArray[contentItem.index] = (byte)contentItem.value.Length;
                }

                Buffer.BlockCopy(relativeLenthsArray, 0, contentArray, offset, relativeLenthsArray.Length);
                offset += relativeLenthsArray.Length;

                foreach (var contentItem in listOfContent.Select((value, index) => new { value, index }))
                {
                    var currentLength = RecognizableLengths[contentItem.index];
                    var currenItem = contentItem.value;
                    currenItem += GetRandomChar(currentLength - currenItem.Length);
                    Buffer.BlockCopy(
                        Encoding.ASCII.GetBytes(currenItem),
                        0, contentArray, offset, currentLength);
                    offset += currentLength;
                }

                MD5 md5 = new()
                {
                    ValueAsByte = contentArray
                };

                var hashResult = md5.Result;
                byte[] controlHashBytes = Encoding.ASCII.GetBytes(hashResult);

                if (controlHashBytes.Length != _controlBytesLength)
                    throw new Exception("Hashing control error");

                Buffer.BlockCopy(contentArray, 0, recognizableArray, 0, contentArray.Length);
                Buffer.BlockCopy(controlHashBytes, 0, recognizableArray, contentArray.Length, _controlBytesLength);
                RecognizableBytesFromEncryption = recognizableArray;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private bool CheckOriginOfFile(byte[] origin)
        {
            byte[] contentBytes = new byte[_contentBytesLength];
            byte[] controlBytes = new byte[_controlBytesLength];
            byte[] relativeLengths = new byte[RecognizableLengths.Count];
            List<string> tempContentArray = new();

            try
            {
                Buffer.BlockCopy(origin, 0, contentBytes, 0, _contentBytesLength);
                Buffer.BlockCopy(origin, _contentBytesLength, controlBytes, 0, _controlBytesLength);
                MD5 md5 = new()
                {
                    ValueAsByte = contentBytes
                };
                byte[] controlHashBytes = Encoding.ASCII.GetBytes(md5.Result);

                if (string.Equals(Encoding.Default.GetString(controlBytes), Encoding.Default.GetString(controlHashBytes)) == false)
                    return false;

                Buffer.BlockCopy(contentBytes, 0, relativeLengths, 0, RecognizableLengths.Count);
                int offset = RecognizableLengths.Count;
                foreach (var contentItem in RecognizableLengths.Select((value, index) => new { value, index }))
                {
                    var tempBytes = contentBytes.Skip(offset).Take(relativeLengths[contentItem.index]).ToArray();
                    tempContentArray.Add(Encoding.Default.GetString(tempBytes));
                    offset += contentItem.value;
                }
                if (Enum.IsDefined(typeof(TypesOfCrypting), tempContentArray[(int)RecognizableElements.Method]))
                {
                    Method = tempContentArray[(int)RecognizableElements.Method];
                    RequestedExtension = tempContentArray[(int)RecognizableElements.Extension];
                    ExtraFileInfo = tempContentArray[(int)RecognizableElements.ExtraInfo];
                    RequestedName = tempContentArray[(int)RecognizableElements.Name];
                    return true;
                }
                throw new Exception("Error while recognizing the file");
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    _safeHandle?.Dispose();
                    GC.Collect();
                }

                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        ~CrytonFile()
        {
            Dispose(false);
        }
    }
}
