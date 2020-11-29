using DiskByte.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiskByte.ViewModel.FolderSelector
{
    class FolderSelectorTreeDirectory : INotifyPropertyChanged
    {
        #region Property Backend Variables
        /// <summary>
        /// <seealso cref="Expanded"/>
        /// </summary>
        private bool _expanded;

        private FolderSelectorTreeDirectory[] _subDirectory;
        #endregion

        #region Properties
        /// <summary>
        /// Parent Directory object.
        /// </summary>
        public FolderSelectorTreeDirectory Parent { get; }

        /// <summary>
        /// Name of the Directory.
        /// </summary>
        public string Name { get => DirInfo.Name; }

        /// <summary>
        /// Info of the Directory. 
        /// </summary>
        public DirectoryInfo DirInfo { get; }

        /// <summary>
        /// Full Path to the Directory.
        /// </summary>
        public string FullPath { get => DirInfo.FullName; }

        /// <summary>
        /// Child Directories.
        /// </summary>
        //public ObservableCollection<FolderSelectorTreeDirectory> Subdirectories { get; }
        public FolderSelectorTreeDirectory[] SubDirectories { get => _subDirectory;
            private set
            {
                if (_subDirectory == value) return;
                _subDirectory = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Dummy child for lazy initialisation. 
        /// </summary>
        private readonly FolderSelectorTreeDirectory _dummyChild;

        /// <summary>
        /// Is the directory expanded. 
        /// Also handles requesting populating the children with real directories. 
        /// </summary>
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (value == _expanded)
                {
                    return;
                }

                _expanded = value;
                if (value)
                {
                    if (SubDirectories.Length == 1 && SubDirectories[0] == _dummyChild)
                    {
                        PopulateChildren();
                    }

                    if (Parent != null)
                    {
                        Parent.Expanded = true;
                    }
                    
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises the FolderSelectorTreeDirectory.
        /// </summary>
        /// <param name="parent">Parent directory in the tree.</param>
        /// <param name="directoryInfo">Directory information of the initialising Directory.</param>
        public FolderSelectorTreeDirectory(FolderSelectorTreeDirectory parent, DirectoryInfo directoryInfo)
        {

            // Set constructor parameters to backend variables. 
            DirInfo = directoryInfo ?? throw new ArgumentNullException();
            Parent = parent;

            //Set up lazy children. 
            _dummyChild = new FolderSelectorTreeDirectory();
            SubDirectories = new FolderSelectorTreeDirectory[] { _dummyChild };
        }

        /// <summary>
        /// Constructor for dummy child. 
        /// </summary>
        private FolderSelectorTreeDirectory()
        {
        }

        #endregion
         
        #region Methods

        /// <summary>
        /// Adds real children to the children collection. 
        /// </summary>
        private void PopulateChildren()
        {
            try
            {
                SubDirectories = DirInfo.GetDirectories().Select(i => new FolderSelectorTreeDirectory(this, i)).ToArray();

            }
            catch (Exception)
            {
                //TODO: Access Denised Error;
            }

        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
