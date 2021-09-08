using System;

namespace CrytonCore.Interfaces
{
    public interface IError
    {
        void GetError(Exception exception);
    }
}
