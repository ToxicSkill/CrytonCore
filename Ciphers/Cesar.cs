using CrytonCore.Abstract;
using CrytonCore.Interfaces;
using CrytonCore.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static CrytonCore.Enums.ETypesOfCrypting;

namespace CrytonCore.Ciphers
{
    public class Cesar : Cipher, ICryptingTools
    {
        private readonly CrytonFile file = new();
        private string name = "Cesar";
        private const int __constRecognizableLenght = 128;

        public override string Name
        {
            get => name;
            set => name = !string.IsNullOrEmpty(value) ? value : "Unknown";
        }

        public Cesar(CrytonFile fileOriginal)
        {
            file = fileOriginal;
            Name = "CESAR";
            file.Method = Name;
        }

        public Cesar() => Name = "Cesar";


        public override async Task<bool> Encrypt(IProgress<int> progress, CancellationToken cancellation)
        {
            return await Task.Run(async () =>
            {
                List<Task> tasks = new();
                int j = 0;

                foreach (var chunk in file.DivData)
                {
                    cancellation.ThrowIfCancellationRequested();
                    ++j;
                    tasks.Add(Task.Run(() => CesarEncryption(chunk)));
                    progress.Report(j);
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);

                return ModifyFile(file.DivData.Count > 0, true);
            }).ConfigureAwait(false);
        }

        private static Task<ParallelLoopResult> CesarEncryption(byte[] chunk)
        {
            return Task.Run(() => Parallel.For(0, chunk.Length, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                chunk[i] += 3;
            }));
        }

        public override async Task<bool> Decrypt(IProgress<int> progress, CancellationToken cancellation)
        {
            return await Task.Run(async () =>
            {
                List<Task> tasks = new();
                int j = 0;

                foreach (var chunk in file.DivData)
                {
                    cancellation.ThrowIfCancellationRequested();
                    ++j;
                    tasks.Add(Task.Run(() => CesarDecryption(chunk)));
                    progress.Report(j);
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
                return ModifyFile(file.DivData.Count > 0, false);
            }).ConfigureAwait(false);
        }
        private static Task<ParallelLoopResult> CesarDecryption(byte[] chunk)
        {
            return Task.Run(() => Parallel.For(0, chunk.Length, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                chunk[i] -= 3;
            }));
        }

        public bool ModifyFile(bool resultOfCrypting, bool status)
        {
            try
            {
                file.MethodId = (int)TypesOfCrypting.CESAR;
                file.Method = EnumToString(TypesOfCrypting.CESAR);
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
