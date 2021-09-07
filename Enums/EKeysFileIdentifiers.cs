using System;

namespace CrytonCore.Enums
{
    public class EKeysFileIdentifiers
    {
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

    }
}
