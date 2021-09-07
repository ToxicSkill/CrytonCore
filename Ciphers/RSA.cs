using CrytonCore.Interfaces;
using System;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using CrytonCore.Model;
using System.Threading;
using static CrytonCore.Enums.ETypesOfCrypting;

namespace CrytonCore.Ciphers
{
    public class RSA : Cipher, ICryptingTools
    {
        private int _bitKeySizeRSA = 512,
            _subChunkSize = 64,
            _subDynamicChunkSize = 128;

        private const int __constChunk = 64,
            __constPartLenght = 3,
            __constRecognizableLenght = 128;

        private struct RSA_Keys
        {
            public BigInteger P;
            public BigInteger Q;
            public BigInteger Phi;
            public BigInteger PxQ;
            public BigInteger PublicKey;
            public BigInteger PrivateKey;
        }

        private RSA_Keys keysRSA = new();
        private string _preparedDecryptionStamp;
        private readonly Random random = new(1410);
        private readonly CrytonFile file = new();
        private string name = "RSA";
        private readonly List<BigInteger> _preparedChunkData_Encryption = new();
        private static byte[] ByteZerosArray;
        private byte[] ByteDynamicArray;
        private readonly List<ValueTuple<int, byte[]>> processedDivData = new();

        public override string Name
        {
            get => name;
            set => name = !string.IsNullOrEmpty(value) ? value : "Unknown";
        }

        public RSA(CrytonFile fileOriginal)
        {
            file = fileOriginal;
            UpdateProperties();
            Name = "RSA";
            file.Method = Name;
        }

        public RSA() => Name = "RSA";

        private void UpdateProperties()
        {
            // (App.Current as App).CrytpingSettings.FirstOrDefault(x => x.GetCryptingMethodName() == TypesOfCrypting.RSA.ToString("g"));
            //_bitKeySizeRSA = Helper.GetSelectedItemAsIntValueFirst() / 2;
            _bitKeySizeRSA = 512 / 2;

            _subChunkSize = _bitKeySizeRSA / 8;
            _subDynamicChunkSize = _bitKeySizeRSA / 4;

            ByteDynamicArray = new byte[_subChunkSize];
            ByteZerosArray = Enumerable.Repeat((byte)0x00, _subDynamicChunkSize).ToArray();
        }

        private struct BIGS
        {
            public BigInteger pqProduct;
            public BigInteger privateKey;
            public BigInteger phi;
        };
        private Task<bool> GenerateRandomKeys()
        {
            Func<bool> function = () =>
            {
                TextWriter tw = new StreamWriter(@"C:\Users\Adam\Desktop\keys.txt");

                List<BIGS> bb = new();
                BIGS temp = new();
                for (int i = 0; i < 100; i++)
                {
                    _ = GenerateRSAKeys().ContinueWith(t => ComputeComponents());
                    temp.pqProduct = keysRSA.PxQ;
                    temp.privateKey = keysRSA.PrivateKey;
                    temp.phi = keysRSA.Phi;
                    bb.Add(temp);
                    Console.WriteLine(i);
                    tw.WriteLine(temp.pqProduct.ToString());
                    tw.WriteLine(temp.privateKey.ToString());
                    tw.WriteLine(temp.phi.ToString());
                }

                tw.Close();
                return true;
            };
            return Task.Run(function);
        }
        public override async Task<bool> Encrypt(IProgress<int> progress, CancellationToken cancellation)
        {
            Stopwatch sw = new();
            sw.Start();

            _ = await GenerateRSAKeys().ContinueWith(t => ComputeComponents());

            if (!CheckKeysStatus())
                return false;

            List<Task<bool>> tasks = new();
            foreach ((byte[] value, int i) item in file.DivData.Select((value, i) => (value, i)))
            {
                try
                {
                    Task<bool> actualTask = Task.Run(() => RSAEncryption(item.value, item.i));
                    tasks.Add(actualTask);
                    cancellation.ThrowIfCancellationRequested();
                    while (await Task.WhenAny(actualTask, Task.Delay(1000)) != actualTask)
                    {
                        progress.Report(item.i);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Error duriing enrypting", item.i);
                    throw;
                }
            }
            _ = await Task.WhenAll(tasks);

            processedDivData.Sort(Comparer<ValueTuple<int, byte[]>>.Default);
            file.DivData.Clear();
            foreach (var item in processedDivData)
            {
                file.DivData.Add(item.Item2);
            }
            if (file.DivData.Count > 0)
                processedDivData.Clear();


            Task resultEvaluation = await TryModInverse().
                ContinueWith(x => GetClipboardString());
            resultEvaluation.Wait();
            if (resultEvaluation.IsCompleted)
            {
                if (file.ClipboardString.Length > 0)
                {
                    bool res = await (System.Windows.Application.Current as App).
                        SaveDataInRelativePath("Keys\\key.dat", new[] {file.ClipboardString });//computing private key - can`t decrypt anything before this line!!!
                }
            }

            sw.Stop();

            Console.WriteLine("Encryption Elapsed={0}", sw.Elapsed);

            _preparedChunkData_Encryption.Clear();

            return ModifyFile(file.DivData.Count > 0, true);
        }

        private bool CheckKeysStatus() => keysRSA.Phi != 0 && keysRSA.PxQ != 0;

        private Task<bool> RSAEncryption(byte[] dataChunk, int chunkIndex)
        {
            return Task.Run(async () =>
            {
                List<byte[]> bigBytes = new();
                List<byte[]> slices = Operation.ArraySplit(dataChunk, _subChunkSize).ToList();
                List<Task<BigInteger>> bigTasks = new();

                if (chunkIndex == file.DivData.Count - 1)
                {
                    file.ExtraFileInfo = slices.Last().Length.ToString();
                }

                foreach (byte[] slice in slices)
                {
                    bigTasks.Add(GetBigIntigerFromBytes(slice));
                }

                BigInteger[] bigs = await Task.WhenAll(bigTasks);

                ParallelQuery<byte[]> computedBytes =
                    from item in bigs.AsParallel()
                    select item != 0 ?
                    ModPowEncryption(item) :
                    ByteZerosArray;

                foreach (var bytes in computedBytes.ToList())
                    bigBytes.Add(bytes);

                computedBytes = null;
                if (bigBytes.Count() > 0)
                {
                    processedDivData.Add(new ValueTuple<int, byte[]>(chunkIndex, bigBytes.SelectMany(x => x).ToArray()));

                    return true;
                }
                return false;
            });
        }
        private Task<BigInteger> GetBigIntigerFromBytes(byte[] slice)
        {
            BigInteger function()
            {
                StringBuilder sb = new();
                foreach (byte i_chunk in slice)
                {
                    _ = sb.Append(((int)i_chunk).ToString("D3"));
                }
                return ToBigInteger(sb.ToString());
            }
            return Task.Run(function);
        }
        private byte[] ModPowEncryption(BigInteger single_data)
        {
            byte[] byteDynamicArray = new byte[_subDynamicChunkSize];
            byte[] tempArr = BigInteger.ModPow(single_data, keysRSA.PublicKey, keysRSA.PxQ).ToByteArray();
            Buffer.BlockCopy(tempArr, 0, byteDynamicArray, 0, tempArr.Length);
            return byteDynamicArray;
        }

        public override async Task<bool> Decrypt(IProgress<int> progress, CancellationToken cancellation)
        {
            return await Task.Run(async () =>
            {
                if (keysRSA.PxQ == null || keysRSA.PxQ == 0 || keysRSA.PrivateKey == 0 || keysRSA.PrivateKey == null)
                {
                    if ((System.Windows.Application.Current as App).AppKeys.numericKeys.Count == 4)
                    {
                        keysRSA.PxQ = (System.Windows.Application.Current as App).AppKeys.numericKeys[1];
                        keysRSA.PrivateKey = (System.Windows.Application.Current as App).AppKeys.numericKeys[2];
                    }
                }
                PrepareDecryptionStamp();

                List<Task<bool>> tasks = new();
                foreach ((byte[] value, int i) item in file.DivData.Select((value, i) => (value, i)))
                {
                    try
                    {
                        Task<bool> actualTask = Task.Run(() => RSADecryption(item.value, item.i));
                        tasks.Add(actualTask);
                        cancellation.ThrowIfCancellationRequested();
                        while (await Task.WhenAny(actualTask, Task.Delay(1000)) != actualTask)
                        {
                            progress.Report(item.i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Error duriing enrypting", item.i);
                        throw;
                    }
                }
                var decryptingResults = await Task.WhenAll(tasks);

                if (decryptingResults.Any(x => x.Equals(false)))
                    return ModifyFile(false, false);

                processedDivData.Sort(Comparer<ValueTuple<int, byte[]>>.Default);
                file.DivData.Clear();
                foreach (var item in processedDivData)
                {
                    file.DivData.Add(item.Item2);
                }
                if (file.DivData.Count > 0)
                    processedDivData.Clear();

                return ModifyFile(file.DivData.Count > 0, false);
            }).ConfigureAwait(false);
        }

        private Task<bool> RSADecryption(byte[] slice, int chunkIndex)
        {
            return Task.Run(() =>
            {
                List<byte[]> preapreBytes = Operation.ArraySplit(slice, _subDynamicChunkSize).ToList();
                List<byte[]> bigBytes = new();

                ParallelQuery<byte[]> matches = from item in preapreBytes.AsParallel()
                                                select Split(BigInteger.ModPow(new BigInteger(item), keysRSA.PrivateKey, keysRSA.PxQ).
                                                ToString(_preparedDecryptionStamp)).ToArray();

                bigBytes = matches.ToList();

                if (chunkIndex == file.DivData.Count - 1)
                {
                    if (int.TryParse(file.ExtraFileInfo, out int extraBytes))
                    {
                        if (extraBytes > 0)
                        {
                            ByteDynamicArray = new byte[extraBytes];
                            Buffer.BlockCopy(bigBytes[bigBytes.Count - 1], _subChunkSize - extraBytes, ByteDynamicArray, 0, extraBytes);
                            bigBytes[bigBytes.Count - 1] = ByteDynamicArray;
                        }
                    }

                }
                //lastExtraBytesArrays.Add(new ValueTuple<int, byte[]>(chunkIndex, bigBytes[bigBytes.Count - 1]));
                if (bigBytes.Count() > 0)
                {
                    processedDivData.Add(new ValueTuple<int, byte[]>(chunkIndex, bigBytes.SelectMany(x => x).ToArray()));
                    return true;
                }
                return false;
            });
        }
        private void PrepareDecryptionStamp()
        {
            StringBuilder stamp = new();
            for (int i = 0; i < _subChunkSize * __constPartLenght; i++)
                _ = stamp.Append("0");
            _preparedDecryptionStamp = stamp.ToString();
        }

        public static IEnumerable<byte> Split(string str) =>
            Enumerable.Range(0, str.Length / __constPartLenght)
            .Select(i => Convert.ToByte(str.Substring(i * __constPartLenght, __constPartLenght)));

        private Task GetClipboardString() =>
            Task.Run(() =>
            file.ClipboardString = "Public Keys: \n" + keysRSA.PublicKey.ToString("x") + "\n" + keysRSA.PxQ.ToString("x")
            + "\nPrivate Keys: " + "\n" + keysRSA.PrivateKey.ToString("x") + "\n" + keysRSA.Phi.ToString("x"));

        private Task<bool> TryModInverse()
        {
            bool function()
            {
                if (keysRSA.PublicKey < 1) throw new ArgumentOutOfRangeException(nameof(keysRSA.PublicKey));
                if (keysRSA.PublicKey < 2) throw new ArgumentOutOfRangeException(nameof(keysRSA.Phi));
                BigInteger n = keysRSA.PublicKey;
                BigInteger m = keysRSA.Phi, v = 0, d = 1;
                while (n > 0)
                {
                    BigInteger t = m / n, x = n;
                    n = m % x;
                    m = x;
                    x = d;
                    d = checked(v - (t * x)); // Just in case
                    v = x;
                }
                keysRSA.PrivateKey = v % keysRSA.Phi;
                if (keysRSA.PrivateKey < 0) keysRSA.PrivateKey += keysRSA.Phi;
                return keysRSA.PublicKey * keysRSA.PrivateKey % keysRSA.Phi == 1L;
            }
            return Task.Run(function);
        }

        public static BigInteger ToBigInteger(string value)
        {
            BigInteger result = 0;
            foreach (char v in value)
                result = result * 10 + (v - '0');
            return result;
        }

        private bool TestComponentsGCD() => keysRSA.P % keysRSA.PublicKey != 1;

        private Task<bool> GenerateRSAKeys()
        {
            Func<bool> function = () =>
            {
                StringBuilder bitKey = new();
                BigInteger bitIntValue;
                bool pqDifferent = false;
                int loopBound = (int)1e5;
                int loopCounter = 0;

                keysRSA.PublicKey = 65537;

                do
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        _ = bitKey.Clear();
                        int pow = _bitKeySizeRSA / __constChunk;
                        _ = bitKey.Append("1");
                        for (int i = 0; i < __constChunk - 2; ++i)
                            _ = bitKey.Append(random.Next(2));
                        _ = bitKey.Append("1");
                        ulong tempBigInt = Convert.ToUInt64(bitKey.ToString(), 2);

                        bitIntValue = BigInteger.Pow(tempBigInt, pow);
                        while (!PrimeTest.IsPrime(bitIntValue))
                        {
                            bitIntValue += 2;
                        }
                        if (j == 0)
                            keysRSA.P = bitIntValue;
                        else
                        {
                            keysRSA.Q = bitIntValue;
                            if (keysRSA.P > 0 && keysRSA.Q > 0 && (keysRSA.P != keysRSA.Q) && TestComponentsGCD())
                            {
                                pqDifferent = true;
                            }
                        }
                    }
                    loopCounter++;
                    if (loopCounter > loopBound)
                        break;
                }
                while (!pqDifferent);

                return pqDifferent && (loopCounter < loopBound);
            };
            return Task.Run(function);
        }

        private bool ComputeComponents()
        {
            keysRSA.PxQ = keysRSA.P * keysRSA.Q;
            keysRSA.Phi = (keysRSA.P - 1) * (keysRSA.Q - 1);
            return keysRSA.Phi > 0 && keysRSA.PxQ > 0;
        }

        private string PrepareControlSum()
        {
            return "21312213123121";
        }

        public bool ModifyFile(bool resultOfCrypting, bool status)
        {
            try
            {
                file.MethodId = (int)TypesOfCrypting.RSA;
                file.Method = EnumToString(TypesOfCrypting.RSA);
                file.Exist = resultOfCrypting;
            }
            catch (Exception)
            {
                return false;
            }
            return file.Status = status;
        }
    
    }
}
