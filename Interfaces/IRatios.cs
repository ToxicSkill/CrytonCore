using System.Collections.Generic;

namespace CrytonCore.Interfaces
{
    internal interface IRatios
    {
        public void SetCurrentRatioByName(string name);
        public void SetCurrentRatioByIndex(int index);
        public List<string> GetRatiosNames();
        public double GetCurrentRatioValue();
        public int GetCurrentRatioIndex();
    }
}
