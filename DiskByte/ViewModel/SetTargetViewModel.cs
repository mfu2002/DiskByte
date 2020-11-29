using DiskByte.Helper;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace DiskByte.ViewModel
{
    class SetTargetViewModel : INotifyPropertyChanged
    {

        #region Properties
        /// <summary>
        /// Did the user press OK (or cancel)?
        /// </summary>
        public bool ResultOk { get; set; }

        internal ResultType DialogResultType = ResultType.AllLogical;

        /// <summary>
        /// All the drives on the user's computer. 
        /// </summary>
        public string[] LogicalDrivesOptions { get; } = Array.Empty<string>();

        /// <summary>
        /// Drives selected by the user in the drives section.
        /// </summary>
        public string[] SelectedDrives { get; set; }

        /// <summary>
        /// Folders selected by the user in the folders section. 
        /// </summary>
        public ObservableCollection<string> SelectedFolders { get; } = new ObservableCollection<string>();


        /// <summary>
        /// Returns paths selected by the user. 
        /// </summary>
        public string[] ResultPaths { get => GetResults(); }


        public RelayCommand<IClosable> OKButtonCommand { get; }
        public RelayCommand<IClosable> CancelButtonCommand { get; }
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the viewmodel. 
        /// </summary>
        public SetTargetViewModel()
        {
            List<string> drives = new List<string>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) { continue; }
                drives.Add(drive.Name);
            }

            LogicalDrivesOptions = drives.ToArray();
            OKButtonCommand = new RelayCommand<IClosable>(OkSubmit, ValidateInfo);
            CancelButtonCommand = new RelayCommand<IClosable>(CancelSubmit);
        }
        #endregion

        #region Methods. 
        /// <summary>
        /// Command Method. User pressed Ok. 
        /// </summary>
        /// <param name="window"></param>
        private void OkSubmit(IClosable window)
        {
            if (window != null)
            {
                ResultOk = true;
                window.Close();
            }
        }

        /// <summary>
        /// Is the selected data on the form valid?
        /// </summary>
        /// <param name="_">Used by OKSubmit Mehtod
        /// <seealso cref="OkSubmit(IClosable)"/>
        /// </param>
        /// <returns></returns>
        private bool ValidateInfo(IClosable _)
        {
            switch (DialogResultType)
            {
                case ResultType.AllLogical:
                    return true;
                case ResultType.SelectedLogical:
                    return (SelectedDrives != null && SelectedDrives.Length > 0);
                case ResultType.SelectedFolders:
                    return SelectedFolders.Count > 0;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Command Methods. User pressed Cancel. 
        /// </summary>
        /// <param name="window"></param>
        private void CancelSubmit(IClosable window)
        {
            if (window != null)
            {
                ResultOk = false;
                window.Close();
            }
        }



        /// <summary>
        /// Adds folder to the selected folders list. 
        /// </summary>
        /// <param name="path"></param>
        internal void AddFolderToList(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                SelectedFolders.Add(path);
            }
        }

        /// <summary>
        /// Removes the folder from the selected folder list.  
        /// </summary>
        /// <param name="folder"></param>
        internal void RemoveFolderFromList(string folder)
        {
            SelectedFolders.Remove(folder);
        }

        /// <summary>
        /// Returns the selected paths from the user. 
        /// </summary>
        /// <returns></returns>
        private string[] GetResults()
        {
            return DialogResultType switch
            {
                ResultType.AllLogical => LogicalDrivesOptions,
                ResultType.SelectedLogical => SelectedDrives,
                ResultType.SelectedFolders => SelectedFolders.OfType<string>().ToArray(),
                _ => LogicalDrivesOptions,
            };
        }


        #endregion


        internal enum ResultType
        {
            AllLogical,
            SelectedLogical,
            SelectedFolders
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
