using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace DiskByte.BusinessLogic
{
    public class Directory : INotifyPropertyChanged
    {
        #region Property Backend Field

        /// <summary>
        /// <seealso cref="Size"/>
        /// </summary>
        private long _size;

        /// <summary>
        /// <seealso cref="FileCount"/>
        /// </summary>
        private int _FileCount;

        /// <summary>
        /// <seealso cref="FolderCount"/>
        /// </summary>
        private int _FolderCount;

        /// <summary>
        /// <seealso cref="DirectoryAnalysing"/>
        /// </summary>
        private bool _directoryAnalysing;

        /// <summary>
        /// <seealso cref="Files"/>
        /// </summary>
        private File[] _files = Array.Empty<File>();
        #endregion

        #region Locks
        private readonly object SubDirectoryLock = new object();
        private readonly object FilesLock = new object();
        #endregion

        #region Property Valid Bools
        internal bool _validSize = false;
        internal bool _validFileCount = false;
        internal bool _validFolderCount = false;
        #endregion

        #region Properties
        /// <summary>
        /// Name of the Directory
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Child Directories
        /// </summary>
        public Directory[] SubDirectories { get; set; } = Array.Empty<Directory>();

        /// <summary>
        /// Child Files
        /// </summary>
        public File[] Files
        {
            get => _files; set
            {
                if (value == _files)
                {
                    return;
                }

                _files = value;

                _validSize = false;
                _validFileCount = false;

                //Properties that changed
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(FileCount));
                OnPropertyChanged(nameof(TotalChildrenCount));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Size of the Directory
        /// </summary>
        public long Size
        {
            get
            {
                if (!_validSize)
                {
                    lock (SubDirectoryLock)
                    {
                        _size = SubDirectories.Sum(x => x?.Size ?? 0);
                    }
                    lock (FilesLock)
                    {
                        _size += (long)Files?.Sum(x => x.Size);
                    }
                    _validSize = true;
                    OnPropertyChanged();
                }
                return _size;
            }
        }

        /// <summary>
        /// Attributes associated to the directory
        /// </summary>
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// When was the directory last changed?
        /// </summary>
        public DateTime LastChanged { get; set; }

        /// <summary>
        /// Total files in the directory and it's subdirectories. 
        /// </summary>
        public int FileCount
        {
            get
            {
                if (!_validFileCount)
                {
                    lock (SubDirectoryLock)
                    {
                        _FileCount = 0;
                        for (int i = 0; i < SubDirectories.Length; i++)
                        {
                            if (SubDirectories[i] == null)
                            {
                                continue;
                            }

                            _FileCount += SubDirectories[i].FileCount;
                        }
                    }
                    lock (FilesLock)
                    {
                        _FileCount += (int)Files?.Length;
                    }
                    _validFileCount = true;
                }
                return _FileCount;
            }
        }

        /// <summary>
        /// Total Folders in the directory and its subdirectories. 
        /// </summary>
        public int FolderCount
        {
            get
            {
                if (!_validFolderCount)
                {
                    lock (SubDirectoryLock)
                    {
                        _FolderCount = 0;
                        for (int i = 0; i < SubDirectories.Length; i++)
                        {
                            if (SubDirectories[i] == null)
                            {
                                continue;
                            }

                            _FolderCount += SubDirectories[i].FolderCount;
                        }
                        _FolderCount += (int)SubDirectories?.Length;
                    }
                    _validFileCount = true;
                }
                return _FolderCount;
            }
        }

        /// <summary>
        /// Total files and subdirectories in the directory and its subdirectories. 
        /// </summary>
        public int TotalChildrenCount { get => FolderCount + FileCount; }
        public bool DirectoryAnalysing
        {
            get => _directoryAnalysing; set
            {
                if (value == _directoryAnalysing)
                {
                    return;
                }

                _directoryAnalysing = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Full path to the directory.
        /// </summary>
        public string FullPath { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Adds the directory to the array and calls the property changed methods. 
        /// </summary>
        /// <param name="dir">Directory that needs to be added. </param>
        /// <param name="position">Index at which it needs to be added</param>
        public void AddDirectory(Directory dir, int position)
        {
            if (SubDirectories == null || position > SubDirectories.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            dir.PropertyChanged += Child_PropertyChanged;

            lock (SubDirectoryLock)
            {
                SubDirectories[position] = dir;
            }
            _validFileCount = false;
            _validFolderCount = false;
            _validSize = false;
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(FileCount));
            OnPropertyChanged(nameof(FolderCount));
            OnPropertyChanged(nameof(TotalChildrenCount));
            OnPropertyChanged(nameof(SubDirectories));
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Sends the Property Changed notification up the chain. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Size)) { _validSize = false; OnPropertyChanged(nameof(Size)); }
            if (e.PropertyName == nameof(FileCount))
            {
                _validFileCount = false;
                OnPropertyChanged(nameof(FileCount));
                OnPropertyChanged(nameof(TotalChildrenCount));
            }
            if (e.PropertyName == nameof(FolderCount))
            {
                _validFolderCount = false;
                OnPropertyChanged(nameof(FolderCount));
                OnPropertyChanged(nameof(TotalChildrenCount));
            }
        }
        #endregion

        #region Constructors
        public Directory()
        {
            //Todo: determine how threadsafe does these two lines make the collection. 
            BindingOperations.EnableCollectionSynchronization(SubDirectories, SubDirectoryLock);
            BindingOperations.EnableCollectionSynchronization(Files, FilesLock);

        }
        #endregion

        #region Equals
        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return ((Directory)obj).FullPath == FullPath;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
