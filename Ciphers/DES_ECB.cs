namespace CrytonCore.Ciphers
{
    //public class DES_ECB : Cipher, ICryptingTools
    //{
    //    readonly CrytonFile file = new CrytonFile();

    //    private string name = "DES_ECB";
    //    public override string Name
    //    {
    //        get { return name; }
    //        set
    //        {
    //            if (!string.IsNullOrEmpty(value))
    //            {
    //                name = value;
    //            }
    //            else
    //            {
    //                name = "Unknown";
    //            };
    //        }
    //    }

    //    public DES_ECB(CrytonFile fileOriginal)
    //    {
    //        file = fileOriginal;
    //        Name = "DES_ECB";
    //    }

    //    public DES_ECB()
    //    {
    //        Name = "DES_ECB";
    //    }

    //    public override async Task<bool> Decrypt()
    //    {
    //        return true;// await Task.Run(() => RunCesarDecryption());
    //    }

    //    public override async Task<bool> Encrypt()
    //    {
    //        return true;// await Task.Run(() => RunCesarEncryption());
    //    }

    //    public bool ModifyFile(bool resultOfCrypting, bool status)
    //    {
    //        try
    //        {
    //            file.MethodId = (int)TypesOfCrypting.DES_ECB;
    //            file.Method = TypesOfCrypting.DES_ECB.ToString();
    //            file.Condition = resultOfCrypting;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //        return file.Status = status;
    //    }
    //}
}
