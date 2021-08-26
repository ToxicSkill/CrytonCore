using System;

namespace CrytonCore.Enums
{
    public class Enumerates
    {
        public enum TypesOfCrypting : int
        {
            CESAR = 0,
            TRANSPOSABLE,
            RSA,
            DES_ECB,
            DES_CBC,
            DES_OFB,
            DES_CFB,
            Triple_DES,
            PASSWORD
        }
        public static string EnumToString(TypesOfCrypting typesOfCrypting)
        {
            return typesOfCrypting switch
            {
                TypesOfCrypting.CESAR => nameof(TypesOfCrypting.CESAR),
                TypesOfCrypting.TRANSPOSABLE => nameof(TypesOfCrypting.TRANSPOSABLE),
                TypesOfCrypting.RSA => nameof(TypesOfCrypting.RSA),
                TypesOfCrypting.DES_ECB => nameof(TypesOfCrypting.DES_ECB),
                TypesOfCrypting.DES_CBC => nameof(TypesOfCrypting.DES_CBC),
                TypesOfCrypting.DES_OFB => nameof(TypesOfCrypting.DES_OFB),
                TypesOfCrypting.DES_CFB => nameof(TypesOfCrypting.DES_CFB),
                TypesOfCrypting.Triple_DES => nameof(TypesOfCrypting.Triple_DES),
                TypesOfCrypting.PASSWORD => nameof(TypesOfCrypting.PASSWORD),
                _ => throw new ArgumentOutOfRangeException(nameof(typesOfCrypting), typesOfCrypting, null),
            };
        }

        public enum KeysFileIdentifiers : int
        {
            PublicTitle = 0,
            PublicKeyFirst,
            PublicKeySecond,
            PrivateTitle,
            PrivateKeyFirst,
            PrivateKeySecond
        }
        public static string EnumToString(KeysFileIdentifiers keysFileIdentifiers)
        {
            return keysFileIdentifiers switch
            {
                KeysFileIdentifiers.PublicTitle => nameof(KeysFileIdentifiers.PublicTitle),
                KeysFileIdentifiers.PublicKeyFirst => nameof(KeysFileIdentifiers.PublicKeyFirst),
                KeysFileIdentifiers.PublicKeySecond => nameof(KeysFileIdentifiers.PublicKeySecond),
                KeysFileIdentifiers.PrivateTitle => nameof(KeysFileIdentifiers.PrivateTitle),
                KeysFileIdentifiers.PrivateKeyFirst => nameof(KeysFileIdentifiers.PrivateKeyFirst),
                KeysFileIdentifiers.PrivateKeySecond => nameof(KeysFileIdentifiers.PrivateKeySecond),
                _ => throw new ArgumentOutOfRangeException(nameof(keysFileIdentifiers), keysFileIdentifiers, null),
            };
        }

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
        public enum ImagesExtensions : int
        {
            jpg,
            jpeg,
            gif,
            png
        }

        public static string EnumToString(ImagesExtensions imagesExtensions)
        {
            return imagesExtensions switch
            {
                ImagesExtensions.jpg => nameof(ImagesExtensions.jpg),
                ImagesExtensions.jpeg => nameof(ImagesExtensions.jpeg),
                ImagesExtensions.gif => nameof(ImagesExtensions.gif),
                ImagesExtensions.png => nameof(ImagesExtensions.png),
                _ => throw new ArgumentOutOfRangeException(nameof(imagesExtensions), imagesExtensions, null),
            };
        }
    }
}
