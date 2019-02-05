using System;
using VeeamAcademy.Archiver.Interfaces;

namespace VeeamAcademy.Archiver.Settings
{
    internal struct Configuration : IConfiguration
    {
        // private readonly int _threadsLimit;

        // _threadsLimit = threadsLimit;

        public int MaxThreadCount => Environment.ProcessorCount;

        public long BufferSize => 128 * 1024 * 1024;
    }
}