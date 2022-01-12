using System.Security;

namespace CrytonCore.Interfaces
{
    internal interface ISecurity
    {
        void SetRandomPassword(int length = 12);
        void SetUserPassword(SecureString secString);
        void SetOwnerPassword(SecureString secString);

    }
}
