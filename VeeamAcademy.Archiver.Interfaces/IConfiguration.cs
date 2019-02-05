namespace VeeamAcademy.Archiver.Interfaces
{
    public interface IArchivationSettings
    {
        long BufferSize { get; }
        int MaxThreadCount { get; }
    }
}