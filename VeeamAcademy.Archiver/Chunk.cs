namespace VeeamAcademy.Archiver
{
    public class Chunk
    {
        public int Id { get; set; }
        public byte[] BufferBytes { get; set; }
        public byte[] ProcessedBytes { get; set; }
    }
}
