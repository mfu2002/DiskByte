
using DiskByte.BusinessLogic;
using DiskByte.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DiskByte.ViewModel.Analyser
{

    class DirectoryGridItem : IAnalyseGridItem, INotifyPropertyChanged
    {

        #region Private variables

        /// <summary>
        /// Backend Directory object that is being analysed or is analysed. 
        /// Holds the main data. 
        /// </summary>
        private readonly Directory _directory;

        /// <summary>
        /// <seealso cref="Expanded"/>
        /// </summary>
        private bool _expanded;

        /// <summary>
        /// holds the reference to the parent directory. 
        /// </summary>
        private readonly DirectoryGridItem _parent;

        /// <summary>
        /// holds the reference to AnalyserGridViewModel that holds the Grid. 
        /// </summary>
        private readonly AnalyserGridViewModel _analyserGridViewModel;

        /// <summary>
        /// Children of the directory as IAnalyseGridItem to hold files and folders. 
        /// </summary>
        private readonly ObservableCollection<IAnalyseGridItem> _children;

        /// <summary>
        /// Used for lazy loading.
        /// </summary>
        private readonly DirectoryGridItem _dummyChild;

        /// <summary>
        /// Size has changed since the last UI update. 
        /// </summary>
        private bool incorrectSize;

        /// <summary>
        /// File count has changed since the last UI update. 
        /// </summary>
        private bool incorrectFileCount;

        /// <summary>
        /// Folder count has changed since the last UI update. 
        /// </summary>
        private bool incorrectFolderCount;

        /// <summary>
        /// Total count has changed since th last UI update. 
        /// </summary>
        private bool incorrectTotalCount;
        #endregion




        #region Constructor
        /// <summary>
        /// Instante a Directory type grid tree item. 
        /// </summary>
        /// <param name="analyserGridViewModel">ViewModel with connection to grid.</param>
        /// <param name="parent">Parent directory</param>
        /// <param name="indent">Directory level.</param>
        /// <param name="directory">Base directory from business logic.</param>
        public DirectoryGridItem(AnalyserGridViewModel analyserGridViewModel, DirectoryGridItem parent, int indent, Directory directory)
        {
            //set internal variables from constructor.
            _analyserGridViewModel = analyserGridViewModel;
            _directory = directory;
            _parent = parent;
            Indent = indent;

            //initiate children. 
            _children = new ObservableCollection<IAnalyseGridItem>();
            _dummyChild = new DirectoryGridItem();
            _children.Add(_dummyChild);

            //Notify when business logic directory object changes. 
            _directory.PropertyChanged += BaseDirectory_PropertyChanged;

            //register UI update timer. 
            GlobalResources.Instance.RefreshTimer.Elapsed += RefreshTimer_Elapsed;

        }

        /// <summary>
        /// Event handler to apply for UI update. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //No need to update UI if the Directory is not being changed. 
            if (!DirectoryAnalysing)
            {
                return;
            }

            if (incorrectSize) { OnPropertyChanged(nameof(Size)); incorrectSize = false; };
            if (incorrectFileCount) { OnPropertyChanged(nameof(FileCount)); incorrectFileCount = false; };
            if (incorrectFolderCount) { OnPropertyChanged(nameof(FolderCount)); incorrectFolderCount = false; };
            if (incorrectTotalCount) { OnPropertyChanged(nameof(TotalCount)); incorrectTotalCount = false; };

        }

        /// <summary>
        /// Handles the change in the base directory. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseDirectory_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Directory.Size))
            {
                incorrectSize = true;
            };
            if (e.PropertyName == nameof(Directory.FileCount))
            {
                incorrectFileCount = true;
            }

            if (e.PropertyName == nameof(Directory.FolderCount))
            {
                incorrectFolderCount = true;
            }

            if (e.PropertyName == nameof(Directory.TotalChildrenCount))
            {
                incorrectTotalCount = true;
            }

            if (e.PropertyName == nameof(Directory.Files))
            {
                ClearChildren();
            }

            if (e.PropertyName == nameof(Directory.SubDirectories))
            {
                ClearChildren();
            }

            if (e.PropertyName == nameof(Directory.DirectoryAnalysing))
            {

                if (DirectoryAnalysing)
                {
                    GlobalResources.Instance.RefreshTimer.Elapsed += RefreshTimer_Elapsed;
                }
                else
                {
                    GlobalResources.Instance.RefreshTimer.Elapsed -= RefreshTimer_Elapsed;
                }
                OnPropertyChanged(nameof(NameColumnText));
                OnPropertyChanged(nameof(DirectoryAnalysing));
            }

        }

        /// <summary>
        /// Resets the children collection back to only having a dummy child. 
        /// </summary>
        private void ClearChildren()
        {
            bool wasExpanded = Expanded;
            Expanded = false;
            _children.Clear();
            _children.Add(_dummyChild);
            if (wasExpanded)
            {
                Expanded = true;
            }
        }


        /// <summary>
        /// Constructor for dummyChild for lazy initialisation. 
        /// </summary>
        private DirectoryGridItem()
        {

        }
        #endregion


        #region Methods

        /// <summary>
        /// Resets the children collection and starts re-analysing the directory files. 
        /// </summary>
        /// <param name="fileItem"></param>
        /// <returns></returns>
        public async Task ChildFileRequestRefresh(FileGridItem fileItem, CancellationToken cancellationToken)
        {
            DirectoryAnalysing = true;

            var fileInfo = new System.IO.FileInfo(fileItem.FullName);
            var parentDirInfo = fileInfo.Directory;
            _directory.Files = await new QueryAnalyser().GetFileArray(parentDirInfo, cancellationToken);
            ClearChildren();

            DirectoryAnalysing = false;
        }

        /// <summary>
        /// deletes the children from the grid and then reinserts them in a sorted order. Lastly recursively calls the SortRefesh on subdirectories. 
        /// </summary>
        public void SortRefresh()
        {
            if (Expanded)
            {
                foreach (var child in _children)
                {
                    _analyserGridViewModel.GridRows.Remove(child);
                }
                InsertChildrenIntoGrid();
                foreach (var child in _children)
                {
                    if (child is DirectoryGridItem subDirectory)
                    {
                        subDirectory.SortRefresh();
                    }
                }
            }
        }
        /// <summary>
        /// Inserts the children into the grid tree after the current directory.
        /// </summary>
        private void InsertChildrenIntoGrid()
        {
            if (IsShowingDummyChild)
            {
                _children.Remove(_dummyChild);
                PopulateChildren();
            }
            var orderedChildren = _children.GetSortedList(_analyserGridViewModel.SortColumn, _analyserGridViewModel.SortOrder);

            var insertIndex = Index + 1;
            foreach (var child in orderedChildren)
            {
                Application.Current.Dispatcher.Invoke(() => _analyserGridViewModel.GridRows.Insert(insertIndex++, child));
            }

        }

        /// <summary>
        /// Returns whether the current children collection holds fake children. 
        /// </summary>
        private bool IsShowingDummyChild => _children.Count == 1 && _children.Contains(_dummyChild);

        /// <summary>
        /// Populates the _children with real children. Used after lazy initialisation. 
        /// </summary>
        private void PopulateChildren()
        {
            for (int i = 0; i < _directory.SubDirectories?.Length; i++)
            {
                if (_directory.SubDirectories[i] == null)
                {
                    continue;
                }

                AddChildDirectory(_directory.SubDirectories[i]);

            }

            for (int i = 0; i < _directory.Files?.Length; i++)
            {
                AddChildFile(_directory.Files[i]);
            }
        }

        /// <summary>
        /// Creates a new DirectoryGridItem and adds it to the children collection. 
        /// </summary>
        /// <param name="dir">Business logic directory. </param>
        private void AddChildDirectory(Directory dir)
        {
            _children.Add(new DirectoryGridItem(_analyserGridViewModel, this, Indent + 1, dir));
        }

        /// <summary>
        /// Creates a new FileGridItem and adds it to the children collection. 
        /// </summary>
        /// <param name="file">Business logic File that is being added.</param>
        private void AddChildFile(File file)
        {
            _children.Add(new FileGridItem(_analyserGridViewModel, this, Indent + 1, file));

        }

        #endregion

        #region IAnalyseGridItem
        /// <summary>
        /// Retuns the string (with the running person and the dropdown icon) as shown on the grid for the name field. 
        /// </summary>
        public string NameColumnText
        {
            get
            {
                //WARNING: The spacing needs to finetuned with the FileGridItem class.
                char expandIcon = ' ';
                string processing = "    ";

                if (Expandable)
                {
                    if (Expanded)
                    {
                        expandIcon = '⮟';
                    }
                    else
                    {
                        expandIcon = '⮞';
                    }
                }
                if (DirectoryAnalysing)
                {
                    processing = "🏃";
                }

                return String.Concat(Enumerable.Repeat("    ", Indent)) + processing + expandIcon + " " + Name;
            }
        }

        /// <summary>
        /// Path of the directory. 
        /// </summary>
        public string FullName => _directory.FullPath;

        /// <summary>
        /// Name of the directory. 
        /// </summary>
        public string Name => _directory.Name;

        /// <summary>
        /// Does Grid have children?
        /// </summary>
        public bool Expandable => _children.Count > 0;

        /// <summary>
        /// Size of the directory. 
        /// </summary>
        public long Size => _directory.Size;

        /// <summary>
        /// When was the directory last changed. 
        /// </summary>
        public DateTime LastChanged => _directory.LastChanged;

        /// <summary>
        /// Are the children of the directory visible on the grid. 
        /// Also handles calls to populate the children collection
        /// and notify the parent to expand. 
        /// </summary>
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded == value)
                {
                    return;
                }

                _expanded = value;
                if (value) //if expanding
                {
                    if (_parent != null)
                    {
                        _parent._expanded = true; //expand the parent recusively all the way up to the root parent. 
                    }

                    //Fill the grid with children.
                    InsertChildrenIntoGrid();

                }
                else  //if Collapsing
                {
                    if (IsShowingDummyChild)
                    {
                        return;
                    }
                    foreach (var child in _children)
                    {
                        //collapse and remove the child from the grid.
                        if (child.Expandable) { child.Expanded = false; }
                        System.Windows.Application.Current.Dispatcher.Invoke(() => _analyserGridViewModel.GridRows.Remove(child));
                    }

                }
                //Notify name expanded gyph changed. 
                OnPropertyChanged(nameof(NameColumnText));
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Index of the directory on the Grid. 
        /// </summary>
        public int Index
        {
            get
            {
                return _analyserGridViewModel.GridRows.IndexOf(this);
            }

        }

        /// <summary>
        /// The depth of the directory in the directory tree. 
        /// </summary>
        public int Indent { get; }

        /// <summary>
        /// Number of Files of the directory and its recursive subdirectories. 
        /// </summary>
        public int FileCount => _directory.FileCount;

        /// <summary>
        /// Number of folders in the directory and its recursive subdirectories.
        /// </summary>
        public int? FolderCount => _directory.FolderCount;

        /// <summary>
        /// aggregate of Filecount and Foldercount. Total number of files and folders in 
        /// the directory and its recursive subdirectories. 
        /// </summary>
        public int TotalCount => _directory.TotalChildrenCount;


        /// <summary>
        /// Reanalysis the directory. 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DataRefresh(CancellationToken cancellationToken)
        {
            QueryAnalyser queryAnalyser = new QueryAnalyser();
            try
            {
                await queryAnalyser.FillInformationAsync(_directory, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                NotificationSystem.Instance.PushNotification("Analysis Cancelled");
            }
        }

        /// <summary>
        /// Is the directory currently being analysed. 
        /// </summary>
        public bool DirectoryAnalysing
        {
            get => _directory.DirectoryAnalysing;
            set
            {
                if (value == _directory.DirectoryAnalysing)
                {
                    return;
                }

                _directory.DirectoryAnalysing = value;
                OnPropertyChanged();
            }
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
