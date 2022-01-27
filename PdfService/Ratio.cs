namespace CrytonCore.PdfService
{
    public class Ratio
    {

        public int CurrentIndex { get; set; }

        public double CurrentValue { get; set; }

        public Ratio() 
        {
            CurrentIndex = 0;
            CurrentIndex = 0;
        }
        public Ratio(int index, double value)
        {
            CurrentIndex = index;
            CurrentValue = value;
        }
        public Ratio(Ratio ratio)
        {
            CurrentIndex = ratio.CurrentIndex;
            CurrentValue = ratio.CurrentValue;
        }
    }
}
