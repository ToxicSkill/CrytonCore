using System;

namespace CrytonCore.Enums
{
    public class ELogger
    {
        public enum Level
        {
            Information = 1,
            Warning = 2,
            Error = 3,
            Fatal = 4,
        }

        public static string EnumToString(Level extensions)
        {
            return extensions switch
            {
                Level.Information => App.GetDictionaryString("Information"),
                Level.Warning => App.GetDictionaryString("Warning"),
                Level.Error => App.GetDictionaryString("Error"),
                Level.Fatal => App.GetDictionaryString("Fatal"),
                _ => throw new ArgumentOutOfRangeException(nameof(extensions), extensions, null),
            };
        }
    }
}
