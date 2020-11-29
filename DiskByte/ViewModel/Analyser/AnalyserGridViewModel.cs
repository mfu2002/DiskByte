using DiskByte.BusinessLogic;
using DiskByte.Helper;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiskByte.ViewModel.Analyser
{
    class AnalyserGridViewModel : INotifyPropertyChanged
    {

        #region Private Variables
        /// <summary>
        /// collection for backend usage (sorting, refresh etc.)
        /// <seealso cref="GridRows">Used for UI changes</seealso>
        /// </summary>
        private readonly ObservableCollection<DirectoryGridItem> _roots = new ObservableCollection<DirectoryGridItem>();
        #endregion

        #region Properties

        /// <summary>
        /// Collection for Grid to display; used by the UI. 
        /// <seealso cref="_roots">Used for backend operations</seealso>
        /// </summary>
        public ObservableCollection<IAnalyseGridItem> GridRows { get; } = new ObservableCollection<IAnalyseGridItem>();

        /// <summary>
        /// Field the grid should be sorted by.
        /// </summary>
        public GridTreeSortColumn SortColumn { get; set; }

        /// <summary>
        /// Ascending or Descending. 
        /// </summary>
        public ListSortDirection SortOrder { get; set; }

        /// <summary>
        /// Selected item on the Grid. 
        /// </summary>
        public IAnalyseGridItem SelectedItem
        {
            get => GlobalResources.Instance.SelectedGridItem; set
            {
                GlobalResources.Instance.SelectedGridItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initiates the Analyser View Model. 
        /// </summary>
        public AnalyserGridViewModel()
        {
            GlobalResources.Instance.PropertyChanged += Global_Instance_PropertyChanged;
        }

        #endregion

        #region Event Handler
        private void Global_Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GlobalResources.AnalysingPaths))
            {
                RemoveAllRoots();
                AddRoots();
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Resets the Analysis page.
        /// </summary>
        public void RemoveAllRoots()
        {

            //data sets cleared. 
            GridRows.Clear();
            _roots.Clear();
        }

        /// <summary>
        /// Adds Root directories to the grid. 
        /// </summary>
        /// <param name="roots"></param>
        /// <returns></returns>
        public void AddRoots()
        {
            foreach (Directory root in GlobalResources.Instance.AnalysingPaths)
            {
                var newRoot = new DirectoryGridItem(this, null, 0, root);
                GridRows.Add(newRoot);
                _roots.Add(newRoot);
            }
        }


        /// <summary>
        /// Deletes the main roots from the datagrid and inserts them in sorted order. Then starts a recursive chain reaction 
        /// that sorts the sub directories in a similar manner (delete and insert in sorted order).
        /// </summary>
        public void SortRefresh()
        {
            foreach (var root in _roots)
            {
                GridRows.Remove(root);
            }
            foreach (var root in _roots.GetSortedList(SortColumn, SortOrder))
            {
                GridRows.Add(root);
            }
            foreach (var root in _roots)
            {
                root.SortRefresh();
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