using DiskByte.Helper;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiskByte.ViewModel.FolderSelector
{
    class FolderSelectorViewModel : INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// List of drives on the system. 
        /// </summary>
        public FolderSelectorTreeDirectory[] Drives { get; private set; }

        /// <summary>
        /// Directory selected on the treeview. 
        /// </summary>
        public FolderSelectorTreeDirectory SelectedDirectory { get; set; }

        /// <summary>
        /// Is the Folder Selector Prompt going to return resuls; i.e. OK or Cancel. 
        /// </summary>
        public bool HasResult { get; internal set; }


        public RelayCommand<IClosable> OKButtonCommand { get; private set; }
        public RelayCommand<IClosable> CancelButtonCommand { get; private set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Initiates a FolderSelectorViewmodel.
        /// </summary>
        public FolderSelectorViewModel()
        {
            List<FolderSelectorTreeDirectory> drives = new List<FolderSelectorTreeDirectory>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if(!drive.IsReady) { continue; }
                drives.Add(new FolderSelectorTreeDirectory(null, new DirectoryInfo(drive.Name)));
            }

            Drives = drives.ToArray();
            OKButtonCommand = new RelayCommand<IClosable>(OKSubmit, ValidateInfo);
            CancelButtonCommand = new RelayCommand<IClosable>(CancelSubmit);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Is the user allowed to press OK? 
        /// </summary>
        /// <param name="_">Needed for OKsubmit function
        /// <seealso cref="OKSubmit(IClosable)"/>
        /// </param>
        /// <returns></returns>
        private bool ValidateInfo(IClosable _)
        {
            return SelectedDirectory != null;
        }

        /// <summary>
        /// Configures to viewmodel to return results; User presssed OK.
        /// </summary>
        /// <param name="window">Closes the window after the ViewModel is configured.</param>
        private void OKSubmit(IClosable window)
        {
            if (window != null)
            {
                HasResult = true;
                window.Close();
            }
        }

        /// <summary>
        /// Configures the viewmodel to not return results; User pressed cancel. 
        /// </summary>
        /// <param name="window">Closes the window after viewmodel is configured. </param>
        private void CancelSubmit(IClosable window)
        {
            if (window != null)
            {
                HasResult = false;
                window.Close();
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
