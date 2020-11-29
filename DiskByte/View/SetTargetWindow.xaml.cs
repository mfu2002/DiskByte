using DiskByte.Helper;
using DiskByte.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiskByte.View
{
    /// <summary>
    /// Interaction logic for SetTargetWindowxaml.xaml
    /// </summary>
    public partial class SetTargetWindow : Window, IClosable
    {
        internal SetTargetViewModel ViewModel { get => (SetTargetViewModel)DataContext; }


        public SetTargetWindow()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {

            FolderSelectorWindow folderSelector = new FolderSelectorWindow();
            folderSelector.Owner = this;
            folderSelector.ShowDialog();
            if (folderSelector.ViewModel.HasResult)
            {
                ViewModel.AddFolderToList(folderSelector.ViewModel.SelectedDirectory.FullPath);
            }
        }


        private void AllLogical_Radio_Checked(object sender, RoutedEventArgs e) => ViewModel.DialogResultType = SetTargetViewModel.ResultType.AllLogical;


        private void SelectedLogical_Radio_Checked(object sender, RoutedEventArgs e) => ViewModel.DialogResultType = SetTargetViewModel.ResultType.SelectedLogical;

        private void SelectedFolders_Radio_Checked(object sender, RoutedEventArgs e) => ViewModel.DialogResultType = SetTargetViewModel.ResultType.SelectedFolders;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();


        private void LstDrives_SelectionChanged(object sender, SelectionChangedEventArgs e) => ViewModel.SelectedDrives = lstDrives.SelectedItems.OfType<string>().ToArray();


        private void RemoveFolder_Click(object sender, RoutedEventArgs e) => ViewModel.RemoveFolderFromList((string)lstFolder.SelectedItem);

    }
}
