using DiskByte.BusinessLogic;
using DiskByte.ViewModel.Analyser;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

namespace DiskByte
{
    public class GlobalResources : INotifyPropertyChanged
    {
        #region Backend Property Variables
        
        /// <summary>
        /// <seealso cref="SelectedGridItem"/>
        /// </summary>
        private IAnalyseGridItem _selectedGridItem;

        /// <summary>
        /// <seealso cref="AnalysingPaths"/>
        /// </summary>
        private Directory[] _analysingPaths = Array.Empty<Directory>();
        
        /// <summary>
        /// <seealso cref="Instance"/> 
        /// </summary>
        private static readonly Lazy<GlobalResources> _instance = new Lazy<GlobalResources>(() => new GlobalResources());

        /// <summary>
        /// <seealso cref="CurrentlyAnalysing"/>
        /// </summary>
        private bool _currentlyAnalysing = false;
        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether an Analysis is currently running. 
        /// </summary>
        public bool CurrentlyAnalysing
        {
            get => _currentlyAnalysing; set
            {
                if (value == _currentlyAnalysing) return;
                _currentlyAnalysing = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Roots of directories being analysed (or have been analysed). 
        /// </summary>
        public Directory[] AnalysingPaths
        {
            get => _analysingPaths; set
            {
                if (_analysingPaths == value)
                {
                    return;
                }

                _analysingPaths = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Item selected on the grid. 
        /// </summary>
        public IAnalyseGridItem SelectedGridItem
        {
            get => _selectedGridItem; set
            {
                if (value == _selectedGridItem)
                {
                    return;
                }

                _selectedGridItem = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Timer used for UI updates/refreshs.
        /// </summary>
        public Timer RefreshTimer { get; } = new Timer(300);

        /// <summary>
        /// Singleton object of this class. 
        /// </summary>
        public static GlobalResources Instance { get { return _instance.Value; } }

        #endregion

        #region Constructors
        /// <summary>
        /// Only used privately for instanciating the singleton. 
        /// </summary>
        private GlobalResources() { RefreshTimer.Start(); }

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
