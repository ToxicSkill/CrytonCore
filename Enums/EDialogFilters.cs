using System;

namespace CrytonCore.Enums
{
    public class EDialogFilters
    {
        public enum DialogFilters
        {
            Jpg = 0,
            Jpeg,
            Png,
            Gif,
            Tiff,
            Pdf,
            All
        }

        public static string EnumToString(DialogFilters dialogFilters)
        {
            return dialogFilters switch
            {
                DialogFilters.Jpg => ExtensionToFilter(nameof(DialogFilters.Jpg)),
                DialogFilters.Jpeg => ExtensionToFilter(nameof(DialogFilters.Jpeg)),
                DialogFilters.Png => ExtensionToFilter(nameof(DialogFilters.Png)),
                DialogFilters.Gif => ExtensionToFilter(nameof(DialogFilters.Gif)),
                DialogFilters.Tiff => ExtensionToFilter(nameof(DialogFilters.Tiff)),
                DialogFilters.Pdf => ExtensionToFilter(nameof(DialogFilters.Pdf)),
                DialogFilters.All => ExtensionToFilter(nameof(DialogFilters.All)),
                _ => throw new ArgumentOutOfRangeException(nameof(dialogFilters), dialogFilters, null),
            };
        }

        private static string ExtensionToFilter(string extension) 
        {
            if (extension == nameof(DialogFilters.All))
                return extension + " files (*.*)|*.*";
            return extension + " file(s) (." + extension + ")|*." + extension; 
        }
        
    }
}
