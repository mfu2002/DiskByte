using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiskByte.ViewModel.Analyser
{
    public interface IAnalyseGridItem
    {

        /// <summary>
        /// Name shown on the grid with the indentation and gyph.
        /// </summary>
        public string NameColumnText { get; }

        /// <summary>
        /// The indentation of the directory or file; Item level in the hierarchy. 
        /// </summary>
        public int Indent { get; }

        /// <summary>
        /// Name of the file or folder. 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Full path of the directory or file.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// States whether the item is expandable. 
        /// </summary>
        public bool Expandable { get; }

        /// <summary>
        /// States whether the object is expanded. 
        /// </summary>
        public bool Expanded { get; set; }

        /// <summary>
        /// The file/folder size on storage medium.
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// The index on the datagrid Tree
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// When was the file/folder last changed/edited. 
        /// </summary>
        public DateTime LastChanged { get; }

        /// <summary>
        /// Number of files in the directory or its children. 
        /// </summary>
        public int FileCount { get; }

        /// <summary>
        /// Number of subdirectories in the directory and its subdirectories.
        /// </summary>
        public int? FolderCount { get; }

        /// <summary>
        /// Total number of folders and files in the directory and its recursive children.
        /// </summary>
        public int TotalCount { get; }


        /// <summary>
        /// Requests the object to reanalyse its data. 
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the task.</param>
        /// <returns></returns>
        public Task DataRefresh(CancellationToken cancellationToken);


    }
}
