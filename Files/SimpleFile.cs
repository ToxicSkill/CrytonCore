namespace CrytonCore.File
{
    public class SimpleFile
    {
        public string Name { get; set; }
        public string SizeString { get; set; }
        public int ChunkSize { get; set; }
        public string Extension { get; set; }
        public string Method { get; set; }
        public int MethodId { get; set; }
        public bool Exist { get; set; }
        public bool Status { get; set; }
        public string ClipboardString { get; set; }
    }
}
