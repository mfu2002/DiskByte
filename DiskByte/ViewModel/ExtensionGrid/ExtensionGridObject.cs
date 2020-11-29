using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace DiskByte.ViewModel.ExtensionGrid
{
    public readonly struct ExtensionGridObject
    {

        /// <summary>
        /// Extension of the files. E.g. .exe, .pdf, etc.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Total size of all the files with this extension. 
        /// </summary>
        public long Size { get; }


        /// <summary>
        /// Construct to set the struct values. 
        /// </summary>
        /// <param name="extension">extension of the files</param>
        /// <param name="size">total size of the files with the given extension.</param>
        public ExtensionGridObject(string extension, long size) =>
            (Extension, Size) = (extension, size);

    }
}
