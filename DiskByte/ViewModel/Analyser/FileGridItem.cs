using DiskByte.BusinessLogic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiskByte.ViewModel.Analyser
{
    class FileGridItem : IAnalyseGridItem
    {

        #region Backend Properties

        /// <summary>
        /// Reference to AnalyserGridViewModel that holds the grid. 
        /// </summary>
        private readonly AnalyserGridViewModel _analyserGridViewModel;

        /// <summary>
        /// Reference to the parent directory. 
        /// </summary>
        private readonly DirectoryGridItem _parent;

        /// <summary>
        /// File backend information. 
        /// </summary>
        private readonly File _file;
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiates a new file type grid item.
        /// </summary>
        /// <param name="analyserGridViewModel">ViewModel with connection to grid.</param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="indent">Directory Level</param>
        /// <param name="file">Base directory from business logic.</param>
        public FileGridItem(AnalyserGridViewModel analyserGridViewModel, DirectoryGridItem parent, int indent, File file)
        {
            Indent = indent;
            _analyserGridViewModel = analyserGridViewModel;
            _parent = parent;
            _file = file;
        }
        #endregion


        #region Properties

        /// <summary>
        /// Exposes the backend file's extension.
        /// </summary>
        public string Extension => _file.Extension;

        #region IAnalyseGridItem
        /// <summary>
        /// Name as visible in the Name column of the Grid.
        /// </summary>
        public string NameColumnText
        {
            get
            {
                return String.Concat(Enumerable.Repeat("    ", Indent)) + "      " + Name;
            }
        }

        /// <summary>
        /// Path the the file. 
        /// </summary>
        public string FullName => _file.FullPath;

        /// <summary>
        /// Name of the file. 
        /// </summary>
        public string Name => _file.Name;

        /// <summary>
        /// File is not expandable. 
        /// </summary>
        public bool Expandable => false;

        /// <summary>
        /// File is always collapsed.
        /// </summary>
        public bool Expanded { get => false; set { } }

        /// <summary>
        /// Size of the file. 
        /// </summary>
        public long Size => _file.Size;

        /// <summary>
        /// Was was the last time the file was changed?
        /// </summary>
        public DateTime LastChanged => _file.LastChanged;

        /// <summary>
        /// Index of the file on the grid. 
        /// </summary>
        public int Index
        {
            get
            {
                return _analyserGridViewModel.GridRows.IndexOf(this);
            }
        }

        /// <summary>
        /// Depth of the file in the directory tree. 
        /// </summary>
        public int Indent { get; }


        /// <summary>
        /// It does not have any chilren therefore, there is only one file (itself). 
        /// </summary>
        public int FileCount => 1;

        /// <summary>
        /// Cannot have folders. 
        /// </summary>
        public int? FolderCount => null;

        /// <summary>
        /// Only one file (itself).
        /// </summary>
        public int TotalCount => 1;


        /// <summary>
        /// requests the parent directory to reanalyse all it's files. 
        /// </summary>
        /// <param name="token">Token to cancel</param>
        /// <returns></returns>
        public Task DataRefresh(CancellationToken token) => _parent.ChildFileRequestRefresh(this, token);
        #endregion

        #endregion
    }
}