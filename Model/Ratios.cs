using CrytonCore.Interfaces;
using System.Collections.Generic;

namespace CrytonCore.Model
{
    internal class Ratios : IRatios
    {
        public List<string> Names { get; init; }

        public List<double> Values { get; init; }

        private static int Count;

        public Ratio CurrentRatio { get; set; }

        public Ratios()
        {
            Names = new List<string>()
            {
                "Original",
                "1.4142 : 1 (A4)",
                "4:3",
                "16:9",
                "1:1",
                "18:9"
            };
            Values = new List<double>()
            {
                0,
                1.414213562373095,
                1.333333333333333,
                1.777777777777777,
                1,
                2
            };

            Count = Values.Count;
            CurrentRatio = new();
        }

        public static int GetCount() => Count;

        public void SetCurrentRatioByName(string name)
        {
            CurrentRatio.CurrentIndex = Names.IndexOf(name);
            CurrentRatio.CurrentValue = Values[CurrentRatio.CurrentIndex];
        }

        public void SetCurrentRatioByIndex(int index)
        {
            CurrentRatio.CurrentIndex = index;
            CurrentRatio.CurrentValue = Values[CurrentRatio.CurrentIndex];
        }

        public List<string> GetRatiosNames()
        {
            return Names;
        }

        public double GetCurrentRatioValue()
        {
            return CurrentRatio.CurrentValue;
        }

        public int GetCurrentRatioIndex()
        {
            return CurrentRatio.CurrentIndex;
        }

        public string GetCurrentRatioName()
        {
            return Names[CurrentRatio.CurrentIndex];
        }

        public string GetRatioNameByIndex(int index)
        {
            if (index < 0 && index > Count)
                return Names[0];
            return Names[index];
        }

        public string GetRatioNameByValue(double value)
        {
            if (value < 0)
                return string.Empty;
            return Names[Values.IndexOf(value)];
        }

        public Ratio GetCurrentRatio()
        {
            return CurrentRatio;
        }
    }

}
