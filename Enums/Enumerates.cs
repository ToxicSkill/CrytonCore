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

        public enum KeysFileIdentifiers : int
        {
            PublicTitle = 0,
            PublicKeyFirst,
            PublicKeySecond,
            PrivateTitle,
            PrivateKeyFirst,
            PrivateKeySecond
        }

        public enum Languages : int
        {
            English = 0,
            Polski,
            French,
            Deutsh
        }

        public enum Themes : int
        {
            Standard = 0,
            Dark,
            Light
        }

        public enum RecognizableElements : int
        {
            Method = 0,
            Extension,
            ExtraInfo,
            Name
        }
        public enum ImagesExtensions : int
        {
            jpg,
            jpeg,
            gif,
            png
        }
    }
}
