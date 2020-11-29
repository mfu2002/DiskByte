using DiskByte.Helper;
using DiskByte.ViewModel.FolderSelector;
using System.Windows;
using System.Windows.Input;

namespace DiskByte.View
{
    /// <summary>
    /// Interaction logic for FolderSelectorWindow.xaml
    /// </summary>
    public partial class FolderSelectorWindow : Window, IClosable
    {
        internal FolderSelectorViewModel ViewModel { get => (FolderSelectorViewModel)DataContext; }
        public FolderSelectorWindow()
        {
            InitializeComponent();
        }

        private void TreeFolder_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedDirectory = (FolderSelectorTreeDirectory)treeFolder.SelectedItem;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HasResult = false;
            Close();
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
