using System;

namespace CrytonCore.Enums
{
    public class EThemes
    {
        public enum Themes : int
        {
            Standard = 0,
            Dark,
            Light
        }
        public static string EnumToString(Themes themes)
        {
            return themes switch
            {
                Themes.Standard => nameof(Themes.Standard),
                Themes.Dark => nameof(Themes.Dark),
                Themes.Light => nameof(Themes.Light),
                _ => throw new ArgumentOutOfRangeException(nameof(themes), themes, null),
            };
        }

    }
}
