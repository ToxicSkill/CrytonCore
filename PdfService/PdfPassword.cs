using System.IO;
using System.Text;

namespace CrytonCore.PdfService
{
    public class PdfPassword
    {
        public string Password { get; private set; }
        public bool HasPassword { get; private set; }

        public PdfPassword()
        {
            Password = string.Empty;
            HasPassword = false;
        }

        public bool SetPassword(string newPassword)
        {
            if(newPassword == null)
                return false;
            if(string.Empty.Equals(newPassword) || newPassword.Length == 0)
                return false;
            Password = newPassword;
            HasPassword = true;
            return true;
        }

        public byte[] GetBytesPassword()
        {
            return HasPassword ? Encoding.ASCII.GetBytes(Password) : null;
        }
    }

    public class PdfPasswordBase
    {
        public string Password { get;  set; }
        public FileInfo Name { get;  set; }

        public PdfPasswordBase(string name)
        {
            Name = new(name);
            Password = string.Empty;    
        }
        public PdfPasswordBase(FileInfo fileInfo)
        {
            Name = fileInfo;
            Password = string.Empty;    
        }
    }
}
