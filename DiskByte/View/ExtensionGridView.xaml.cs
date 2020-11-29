using DiskByte.Helper;
using DiskByte.ViewModel.ExtensionGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiskByte.View
{
    /// <summary>
    /// Interaction logic for ExtensionGridView.xaml
    /// </summary>
    public partial class ExtensionGridView : UserControl
    {
        ExtensionGridViewModel ViewModel => (ExtensionGridViewModel)DataContext;
        public ExtensionGridView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Brings the selection in view and ensures it is highlighted. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);


                //Set to null then back to the actual value to force the grid to highlight the row if it was initially out of view. 
                var highlightTemp = dataGrid.SelectedItem;
                dataGrid.SelectedItem = null;
                dataGrid.SelectedItem = highlightTemp;
            }
        }

        /// <summary>
        /// Sets up the sorting settings and runs the Sorting method in the viewmodel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            ViewModel.SortingColumn = e.Column.Header switch
            {
                "Extension" => ExtensionGridSortColumn.Extension,
                _ => ExtensionGridSortColumn.Size,
            };

            var newSortOrder = ListSortDirection.Ascending;

            if (e.Column.SortDirection != null && e.Column.SortDirection == ListSortDirection.Ascending)
            {
                newSortOrder = ListSortDirection.Descending;
            }



            e.Column.SortDirection = newSortOrder;
            ViewModel.SortOrder = newSortOrder;

            ViewModel.Sort();
            e.Handled = true;

        }
    }
}
