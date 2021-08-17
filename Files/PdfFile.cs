namespace CrytonCore.Files
{
    public class PdfFile
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string UserPassword { get; set; }
        public string OwnerPassword { get; set; }
        public uint Size { get; set; }
        public bool UserOwnerPassword { get; set; }
        public bool HighestEncryptionLevel { get; set; }
        public bool RandomPassword { get; set; }
        public void Clear()
        {
            Url = string.Empty;
            Name = string.Empty;
        }

        public bool IsEmpty() => Name.Length > 0;
    }
}
