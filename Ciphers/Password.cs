using System;

namespace CrytonCore.Ciphers
{
    public class Password //: Cipher, ICryptingTools
    {
        private string name = "Password";
        //public override string Name
        //{
        //    get { return name; }
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            name = value;
        //        }
        //        else
        //        {
        //            name = "Unknown";
        //        };
        //    }
        //}

        //public Password()
        //{
        //    Name = "Password";
        //}

        //public override bool Decrypt()
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool Encrypt()
        //{
        //    throw new NotImplementedException();
        //}

        public bool ModifyFile(bool resultOfCrypting, bool encORdec)
        {
            throw new NotImplementedException();
        }
    }
}
