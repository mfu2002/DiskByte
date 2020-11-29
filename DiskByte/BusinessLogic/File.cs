using System;
using System.IO;

namespace DiskByte.BusinessLogic
{


    public readonly struct File
    {
        #region Properties

        /// <summary>
        /// Name of the file. 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Size in bytes of the File. 
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// When was the File as changed. 
        /// </summary>
        public DateTime LastChanged { get; }

        /// <summary>
        /// Attributes associated to the file.
        /// </summary>
        public FileAttributes Attributes { get; }

        /// <summary>
        /// Full Path to the file. 
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Extension of the file.E.g. Exe, Pdf etc. 
        /// </summary>
        public string Extension { get; }

        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate the struct
        /// </summary>
        /// <param name="name">Name of the file.</param>
        /// <param name="fullPath">Full path to the file.</param>
        /// <param name="size">Size of the file in bytes.</param>
        /// <param name="lastChanged">When was the file last changed?</param>
        /// <param name="attr">Attributes associated to the file.</param>
        public File(string name, string extension,  string fullPath, long size, DateTime lastChanged, FileAttributes attr) =>
            (Name, Extension,  Size, LastChanged, Attributes, FullPath) = (name, extension, size, lastChanged, attr, fullPath);
        #endregion
    }
}
