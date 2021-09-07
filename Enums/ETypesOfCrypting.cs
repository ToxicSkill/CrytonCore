using System;

namespace CrytonCore.Enums
{
    public class ETypesOfCrypting
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
    }
}