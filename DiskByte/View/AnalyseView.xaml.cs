using DiskByte.Helper;
using DiskByte.ViewModel.Analyser;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiskByte.View
{
    /// <summary>
    /// Interaction logic for AnalyseControl.xaml
    /// </summary>
    public partial class AnalyseView : UserControl
    {
        internal AnalyserGridViewModel ViewModel => (AnalyserGridViewModel)DataContext;

        public AnalyseView()
        {
            InitializeComponent();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DirectoryGridTree.SelectedItem is IAnalyseGridItem selectedRow)
            {
                if (selectedRow.Expandable)
                {
                    selectedRow.Expanded = !selectedRow.Expanded;
                }
            }

        }

        private void DirectoryGridTree_Sorting(object sender, DataGridSortingEventArgs e)
        {
            ViewModel.SortColumn = e.Column.Header switch
            {
                "Name" => GridTreeSortColumn.Name,
                "Last Changed" => GridTreeSortColumn.LastChanged,
                "Size" => GridTreeSortColumn.Size,
                "File Count" => GridTreeSortColumn.FileCount,
                "Directory Count" => GridTreeSortColumn.DirectoryCount,
                "Total Count" => GridTreeSortColumn.TotalCount,
                _ => GridTreeSortColumn.Name,
            };

            var newSortOrder = ListSortDirection.Ascending;

            if (e.Column.SortDirection != null && e.Column.SortDirection == ListSortDirection.Ascending)
            {
                newSortOrder = ListSortDirection.Descending;
            }
            e.Column.SortDirection = newSortOrder;


            ViewModel.SortOrder = newSortOrder;

            ViewModel.SortRefresh();


            e.Handled = true;
        }
    }
}
