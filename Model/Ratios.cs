using CrytonCore.Interfaces;
using System.Collections.Generic;

namespace CrytonCore.Model
{
    internal class Ratios : IRatios
    {
        public List<string> Names { get; init; }

        public List<double> Values { get; init; }

        private static int Count;

        public int CurrentIndex { get; set; }

        public double CurrentValue { get; set; }

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
        }

        public static int GetCount() => Count;

        public void SetCurrentRatioByName(string name)
        {
            CurrentIndex = Names.IndexOf(name);
            CurrentValue = Values[CurrentIndex];
        }

        public void SetCurrentRatioByIndex(int index)
        {
            CurrentIndex = index;
            CurrentValue = Values[CurrentIndex];
        }

        public List<string> GetRatiosNames()
        {
            return Names;
        }

        public double GetCurrentRatioValue()
        {
            return CurrentValue;
        }

        public int GetCurrentRatioIndex()
        {
            return CurrentIndex;
        }
    }

}
