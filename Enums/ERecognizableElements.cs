using System;

namespace CrytonCore.Enums
{
    public class ERecognizableElements
    {
        public enum RecognizableElements : int
        {
            Method = 0,
            Extension,
            ExtraInfo,
            Name
        }

        public static string EnumToString(RecognizableElements recognizableElements)
        {
            return recognizableElements switch
            {
                RecognizableElements.Method => nameof(RecognizableElements.Method),
                RecognizableElements.Extension => nameof(RecognizableElements.Extension),
                RecognizableElements.ExtraInfo => nameof(RecognizableElements.ExtraInfo),
                RecognizableElements.Name => nameof(RecognizableElements.Name),
                _ => throw new ArgumentOutOfRangeException(nameof(recognizableElements), recognizableElements, null),
            };
        }
    }
}
