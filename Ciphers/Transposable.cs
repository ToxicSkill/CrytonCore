using CrytonCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using CrytonCore.Enums;
using System.Threading.Tasks;
using System.Threading;
using CrytonCore.Model;
using static CrytonCore.Enums.ETypesOfCrypting;

namespace CrytonCore.Ciphers
{
    public class Transposable : Cipher, ICryptingTools
    {
        readonly CrytonFile file = new();
        private string name = "Transposable";

        public override string Name
        {
            get { return name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    name = value;
                }
                else
                {
                    name = "Unknown";
                };
            }
        }

        public Transposable(CrytonFile fileOriginal)
        {
            file = fileOriginal;
        }
        public Transposable()
        {
            Name = "Transposable";
        }

        public override async Task<bool> Decrypt(IProgress<int> progress, CancellationToken cancellation)
        {
            //int j = 0;

            //List<Task<bool>> tasks = new List<Task<bool>>();

            //    Parallel.ForEach<byte[]>(file.DivData, (chunk) =>
            //    {
            //        tasks.Add(Task.Run(() => SnailDecryption(chunk)));
            //        progress.Report(j);
            //        ++j;
            //    });
            //await Task.WhenAll(tasks);
            _ = await TransposableDecryption(file.DivData
                .Where((v, i) => i != file.DivData.Count - 1).ToList(), progress)
                .ConfigureAwait(false);

            return ModifyFile(file.DivData.Count > 0, false);
        }

        public override async Task<bool> Encrypt(IProgress<int> progress, CancellationToken cancellation)
        {
            if (file.Size < 5) 
                return false;

            List<Task> tasks = new();

            int sizeOfDivData = file.DivData.Count;
            var lastIndex = file.DivData.Count - 1;
            var temp = file.DivData[lastIndex];

            //foreach (var chunk in file.DivData)
            //{
            //    tasks.Add(SnailEncryption(chunk));
            //    progress.Report(j);
            //    ++j;
            //}
            //await Task.WhenAll(tasks);
            _ = await TransposableEncryption(file.DivData
                .Where((v, i) => i != file.DivData.Count - 1).ToList(), progress)
                .ConfigureAwait(false);
            //await Task.Run(() =>
            //{
            //Parallel.ForEach<byte[]>(file.DivData, (chunk) =>
            //{
            //    tasks.Add(Task.Run(() => SnailEncryption(chunk)));
            //    progress.Report(j);
            //    ++j;
            //});
            //}).ContinueWith(t =>
            //{
            //    matrixDimension = Math.Ceiling(Math.Pow(file.DivData[lastIndex].Length, 0.5));
            //    file.DivData[lastIndex] = new byte[(int)(matrixDimension * matrixDimension)];
            //    Array.Copy(temp, 0, file.DivData[lastIndex], 0, temp.Length);
            //    SnailEncryption(file.DivData[lastIndex]);
            //    progress.Report(sizeOfDivData);
            //});
            //.ContinueWith(t =>
            //{
            //    matrixDimension = Math.Ceiling(Math.Pow(file.DivData[lastIndex].Length, 0.5));
            //    file.DivData[lastIndex] = new byte[(int)(matrixDimension * matrixDimension)];
            //    Array.Copy(temp, 0, file.DivData[lastIndex], 0, temp.Length);
            //    SnailEncryption(file.DivData[lastIndex]);
            //    progress.Report(sizeOfDivData);
            //});

            //matrixDimension = Math.Ceiling(Math.Pow(file.DivData[lastIndex].Length, 0.5));
            //file.DivData[lastIndex] = new byte[(int)(matrixDimension * matrixDimension)];
            //Array.Copy(temp, 0, file.DivData[lastIndex], 0, temp.Length);
            //SnailEncryption(file.DivData[lastIndex]);
            //progress.Report(sizeOfDivData);

            return ModifyFile(file.DivData.Count > 0, true);
        }

        //private int GetOversize()
        //{
        //    string oversize = default(string);
        //    int i = 0;

        //    while (file.Data[i] != '\n')
        //    {
        //        oversize = oversize + (char)file.Data[i];
        //        ++i;
        //    }

        //    if (file.Data[i + 2] == 13)
        //        ++i;

        //    file.Data = Operation.RemoveByteFromBegArray(file.Data, i + 1);

        //    Int32.TryParse(oversize, out i);

        //    return i;
        //}


        private static Task<ParallelLoopResult> TransposableEncryption(List<byte[]> chunks, IProgress<int> progress)
        {
            int progressCounter = 0;
            return Task.Run(() => Parallel.ForEach(chunks,new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                ++progressCounter;
                SnailEncryption(i);
                    progress.Report(progressCounter);
            }));
        }

        private static Task<ParallelLoopResult> TransposableDecryption(List<byte[]> chunks, IProgress<int> progress)
        {
            int progressCounter = 0;
            return Task.Run(() => Parallel.ForEach(chunks, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                ++progressCounter;
                SnailDecryption(i);
                progress.Report(progressCounter);
            }));
        }

        private static void SnailEncryption(byte[] chunk)
        {
            List<char> encryptedSnail = new();
            List<char> temp = new();
            List<List<char>> encryptedColumn = new();

            var size = chunk.Length;
            var matrixDimension = Math.Ceiling(Math.Pow(size, 0.5));

            var overflow = Math.Pow(matrixDimension, 2) - chunk.Length;

            char[,] Matrix = new char[(int)matrixDimension, (int)matrixDimension];

            var tempChunk = chunk;

            if (overflow > 0)
                chunk = new byte[(int)(matrixDimension * matrixDimension)];

            Array.Copy(tempChunk, 0, chunk, 0, tempChunk.Length);

            var it = 0;
            for (var i = 0; i < matrixDimension; ++i)
            {
                for (var j = 0; j < matrixDimension; ++j)
                {
                    Matrix[i, j] = (char)chunk[it];
                    ++it;
                }
            }

            int limit;
            long s = (long)matrixDimension;
            if ((matrixDimension % 2) == 1)
                limit = (int)(matrixDimension - 1);
            else
                limit = (int)(matrixDimension - 2);

            for (var cntr = 0; cntr < limit; ++cntr)
            {
                for (var i = cntr; i < matrixDimension - cntr; ++i)
                    encryptedSnail.Add(Matrix[cntr, i]);

                for (var i = cntr + 1; i < matrixDimension - cntr - 1; ++i)
                    encryptedSnail.Add(Matrix[i, s - cntr - 1]);

                for (long i = (long)(matrixDimension - 1 - cntr); i >= cntr; --i)
                    encryptedSnail.Add(Matrix[s - cntr - 1, i]);

                for (long i = (long)(matrixDimension - 1 - cntr); i > cntr + 1; --i)
                    encryptedSnail.Add(Matrix[i - 1, cntr]);
            }

            if ((matrixDimension % 2) == 1)
                _ = encryptedSnail.Remove((char)(encryptedSnail.Count - 1));

            for (var i = 0; i < encryptedSnail.Count; ++i)
            {
                temp.Add(encryptedSnail[i]);
                encryptedSnail[i] = default;
                if (temp.Count == matrixDimension)
                {
                    encryptedColumn.Add(new List<char>(temp));
                    temp.Clear();
                }
            }

            if (encryptedColumn.Count > 0)
            {
                Array.Clear(chunk, 0, chunk.Length);

                it = 0;
                for (var i = 0; i < matrixDimension; ++i)
                    for (var j = 0; j < matrixDimension; ++j)
                    {
                        chunk[it] = (byte)encryptedColumn[j][i];
                        ++it;
                    }
                encryptedColumn.Clear();
            }
            encryptedSnail.Clear();
        }

        private static void SnailDecryption(byte[] chunk)
        {
            List<char> encryptedSnail = new();

            var size = chunk.Length;
            var matrixDimension = Math.Ceiling(Math.Pow(size, 0.5));

            var overflow = Math.Pow(matrixDimension, 2) - chunk.Length;

            char[,] Matrix = new char[(int)matrixDimension, (int)matrixDimension];

            List<byte[]> temp = new(ArraySplit(chunk, (int)matrixDimension));

            for (var i = 0; i < matrixDimension; ++i)
                for (var j = 0; j < matrixDimension; ++j)
                    encryptedSnail.Add((char)temp[j][i]);

            int iterator = 0;
            long s = (long)matrixDimension;
            int limit;
            if ((matrixDimension % 2) == 1)
                limit = (int)((matrixDimension - 1) / 2);
            else
                limit = (int)(matrixDimension / 2);

            for (var cntr = 0; cntr < limit; ++cntr)
            {
                for (var i = (long)cntr; i < matrixDimension - cntr; ++i)
                {
                    Matrix[cntr, i] = encryptedSnail[iterator];
                    ++iterator;
                }

                for (var i = (long)(cntr + 1); i < matrixDimension - cntr - 1; ++i)
                {
                    Matrix[i, s - cntr - 1] = encryptedSnail[iterator];
                    ++iterator;
                }

                for (var i = (long)(matrixDimension - 1 - cntr); i >= cntr; --i)
                {
                    Matrix[s - cntr - 1, i] = encryptedSnail[iterator];
                    ++iterator;
                }

                for (var i = (long)(matrixDimension - 1 - cntr); i > cntr + 1; --i)
                {
                    Matrix[i - 1, cntr] = encryptedSnail[iterator];
                    ++iterator;
                }
            }
            if ((matrixDimension % 2) == 1)
                Matrix[(int)((matrixDimension - 1) / 2),(int)( (matrixDimension - 1) / 2)] = encryptedSnail[iterator];

            encryptedSnail.Clear();

            s = 0;

            for (var i = 0; i < matrixDimension; ++i)
                for (var j = 0; j < matrixDimension; ++j)
                {
                    chunk[s] = (byte)Matrix[i, j];
                    ++s;
                }

        }

        private static IEnumerable<byte[]> ArraySplit(byte[] bArray, int intBufforLengt)
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

        private string PrepareControlSum()
        {
            return "2112";
        }

        public bool ModifyFile(bool resultOfCrypting, bool status)
        {
            try
            {
                file.MethodId = (int)TypesOfCrypting.TRANSPOSABLE;
                file.Method = EnumToString(TypesOfCrypting.TRANSPOSABLE);
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
