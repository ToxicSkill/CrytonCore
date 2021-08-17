using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrytonCore.Interfaces
{
    public interface IError
    {
        void GetError(Exception exception);
    }
}
