using System;

namespace CrytonCore.Enums
{
    public class ELanguages
    {
        public enum Languages : int
        {
            English = 0,
            Polski,
            French,
            Deutsh
        }
        public static string EnumToString(Languages languages)
        {
            return languages switch
            {
                Languages.English => nameof(Languages.English),
                Languages.Polski => nameof(Languages.Polski),
                Languages.Deutsh => nameof(Languages.Deutsh),
                Languages.French => nameof(Languages.French),
                _ => throw new ArgumentOutOfRangeException(nameof(languages), languages, null),
            };
        }
    }
}
