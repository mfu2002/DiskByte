using DiskByte.BusinessLogic;
using DiskByte.Helper;
using DiskByte.ViewModel.Analyser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiskByte.ViewModel.ExtensionGrid
{
   public class ExtensionGridViewModel: INotifyPropertyChanged
    {

        #region Property Backend Variables


        /// <summary>
        /// <seealso cref="SelectedRow"/>
        /// </summary>
        private ExtensionGridObject _selectedRow;


        #endregion

        #region Properties
        /// <summary>
        /// Array of Extension rows views on the datagrid. 
        /// </summary>
        public ObservableCollection<ExtensionGridObject> Rows { get; } = new ObservableCollection<ExtensionGridObject>();

        /// <summary>
        /// Row selected on the extension datagrid.
        /// </summary>
        public ExtensionGridObject SelectedRow
        {
            get => _selectedRow; 
            set
            {
                _selectedRow = value;
                OnPropertyChanged();

            }
        }

        /// <summary>
        /// Column Extension data grid is sorted by. 
        /// </summary>
        public ExtensionGridSortColumn SortingColumn { get; set; } = ExtensionGridSortColumn.Size;


        /// <summary>
        /// Ascending or Descending
        /// </summary>
        public ListSortDirection SortOrder { get; set; } = ListSortDirection.Descending;

        #endregion



        /// <summary>
        /// creates an instance of the viewmodel.
        /// </summary>
        public ExtensionGridViewModel()
        {
            GlobalResources.Instance.PropertyChanged += Global_Instance_PropertyChanged;
        }


        #region Event Handlers

        /// <summary>
        /// Listens to changes in the Global Resources singleton. 
        /// <seealso cref="GlobalResources"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Global_Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GlobalResources.CurrentlyAnalysing) && !GlobalResources.Instance.CurrentlyAnalysing)
            {

                await Task.Run(() => PopulateRows());

            }
            if (e.PropertyName == nameof(GlobalResources.SelectedGridItem))
            {
                ReEvaluateSelected();
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Sorts the grid by the parameters establised in <see cref="SortOrder"/> and <see cref="SortingColumn"/>
        /// </summary>
        public void Sort()
        {
            List<ExtensionGridObject> SortedList = Rows.ToList();
            if (SortingColumn == ExtensionGridSortColumn.Extension)
            {
                
                if (SortOrder == ListSortDirection.Descending)
                {
                    SortedList = SortedList.OrderByDescending(r => r.Size).ToList();
                }
                else
                {
                    SortedList = Rows.OrderBy(r => r.Extension).ToList();
                }
            }
            else
            {
                if (SortOrder == ListSortDirection.Descending)
                {
                    SortedList = Rows.OrderByDescending(r => r.Size).ToList();
                }
                else
                {
                    SortedList = Rows.OrderBy(r => r.Size).ToList();
                }
            }

            Rows.Clear();

            SortedList.ForEach(i => Rows.Add(i));

        }



        /// <summary>
        /// Selects the extension of the file selected on the Main Grid (if a file is selected)
        /// </summary>
        private void ReEvaluateSelected()
        {
            if (GlobalResources.Instance.SelectedGridItem is FileGridItem GridFile)
            {
              SelectedRow = Rows.First(i => i.Extension == GridFile.Extension);
            }
        }


        /// <summary>
        /// Populates the <see cref="Rows">Rows</see> collection with extension and size.
        /// </summary>
        private void PopulateRows()
        {
            Dictionary<string, long> extensionSizePair = new Dictionary<string, long>();

            foreach (var dir in GlobalResources.Instance.AnalysingPaths)
            {
                GetFiles(dir, extensionSizePair);
            }

            foreach (var pair in extensionSizePair)
            {
                Application.Current.Dispatcher.Invoke(()=>Rows.Add(new ExtensionGridObject(pair.Key, pair.Value)));
            }
        }

        /// <summary>
        /// recursively fills the dictionary with extension and size. 
        /// </summary>
        /// <param name="directory">Root directory to evaluate.</param>
        /// <param name="extensionSizePair">Dictionary to fill the information in.</param>
        private void GetFiles(Directory directory, Dictionary<string, long> extensionSizePair)
        {
            
            foreach (var file in directory.Files)
            {
                if (!extensionSizePair.ContainsKey(file.Extension))
                {
                    extensionSizePair.Add(file.Extension, 0);
                }

                extensionSizePair[file.Extension] += file.Size;
            }


            foreach (var dir in directory.SubDirectories)
            {
                GetFiles(dir, extensionSizePair);
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
