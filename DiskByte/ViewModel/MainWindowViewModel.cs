using DiskByte.BusinessLogic;
using DiskByte.Helper;
using DiskByte.View;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DiskByte.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Private Variables
        /// <summary>
        /// Token for cancelling the analysis (or refresh).
        /// </summary>
        private CancellationTokenSource cancellationToken;

        /// <summary>
        /// Saves the last assigned value to the ExtensionColumnWidth.
        /// Used for when the Extension datagrid is hidden. 
        /// </summary>
        private GridLength lastExtensionColumnWidth = new GridLength(250, GridUnitType.Pixel);

        /// <summary>
        /// <seealso cref="ExtensionColumnWidth"/>
        /// </summary>
        private GridLength _extensionColumnWidth = new GridLength(0, GridUnitType.Pixel);

        #endregion

        #region Properties
        public RelayCommand<Window> MenuOpenButtonClicked { get; }
        public RelayCommand DataRefeshButton { get; }

        /// <summary>
        /// Turnel to the NotificationSystem instance for binding from the view. 
        /// </summary>
        public NotificationSystem NotificationSystem => NotificationSystem.Instance;


        /// <summary>
        /// Width of the Extension Data Grid column. 
        /// </summary>
        public GridLength ExtensionColumnWidth { get => _extensionColumnWidth; set
            {
                if (_extensionColumnWidth == value) return;

                //Values should not be set to anything other than zero while the analysis is running. 
                if (GlobalResources.Instance.CurrentlyAnalysing && value.Value != 0) return;

                if (!(GlobalResources.Instance.CurrentlyAnalysing && value.Value == 0))
                {
                    lastExtensionColumnWidth = value;
                }
                _extensionColumnWidth = value;

                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a instance of MainWindowViewModel.
        /// </summary>
        public MainWindowViewModel()
        {
            //Instantiates the commands.
            MenuOpenButtonClicked = new RelayCommand<Window>(GetPathsToAnalyse);
            DataRefeshButton = new RelayCommand(DataRefresh, () => GlobalResources.Instance.SelectedGridItem != null && cancellationToken == null);

            //subscribes to the neccessary events. 
            GlobalResources.Instance.PropertyChanged += Global_Instance_PropertyChanged;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles change in Global singleton variabes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Global_Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GlobalResources.AnalysingPaths))
            {
                //Task.Run used for CPU intensive jobs. 
                await Task.Run(() => StartAnalysis());
            }

            if (e.PropertyName == nameof(GlobalResources.CurrentlyAnalysing))
            {
                if (GlobalResources.Instance.CurrentlyAnalysing)
                {
                    ExtensionColumnWidth = new GridLength(0);
                }
                else
                {
                    ExtensionColumnWidth = lastExtensionColumnWidth;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Forces the selected item to re-analyse its children. 
        /// </summary>
        public async void DataRefresh()
        {
            //Cancels analysis if it is running. (it shouldn't be). 
            if (cancellationToken != null) { cancellationToken.Cancel(); }

            NotificationSystem.Instance.PushNotification("Rechecking...");
            cancellationToken = new CancellationTokenSource();
            try
            {
                //Task.Run used for CPU intensive jobs. 
                GlobalResources.Instance.CurrentlyAnalysing = true;
                await Task.Run(() => GlobalResources.Instance.SelectedGridItem.DataRefresh(cancellationToken.Token)).ConfigureAwait(false);
                cancellationToken = null;
                NotificationSystem.Instance.PushNotification("Analysis complete.");
            }
            catch (OperationCanceledException)
            {
                NotificationSystem.Instance.PushNotification("Analysis cancelled.");
            }
            finally
            {
                GlobalResources.Instance.CurrentlyAnalysing = false;

            }
        }

        /// <summary>
        /// Analysis the directories. 
        /// </summary>
        /// <returns></returns>
        private async Task StartAnalysis()
        {
            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
            }

            cancellationToken = new CancellationTokenSource();

            NotificationSystem.Instance.PushNotification("Started analysing...");
            List<Task> analysisTasks = new List<Task>();

            GlobalResources.Instance.CurrentlyAnalysing = true;
            QueryAnalyser queryAnalyser = new QueryAnalyser();
            foreach (Directory root in GlobalResources.Instance.AnalysingPaths)
            {
                analysisTasks.Add(queryAnalyser.FillInformationAsync(root, cancellationToken.Token));
            }

            try
            {
                await Task.WhenAll(analysisTasks).ConfigureAwait(false);
                cancellationToken = null;
                NotificationSystem.Instance.PushNotification("Analysis complete.");
            }
            catch (OperationCanceledException)
            {
                NotificationSystem.Instance.PushNotification("Analysis cancelled.");
            }
            finally
            {
                GlobalResources.Instance.CurrentlyAnalysing = false;

            }
        }


        /// <summary>
        /// Prompts the user to select paths to analyse. 
        /// </summary>
        /// <param name="window"></param>
        public void GetPathsToAnalyse(Window window)
        {
            SetTargetWindow setTargetWindow = new SetTargetWindow() { Owner = window };
            setTargetWindow.ShowDialog();
            if (setTargetWindow.ViewModel.ResultOk)
            {

                List<Directory> results = new List<Directory>();
                setTargetWindow.ViewModel.ResultPaths.ForEach(x => results.Add(new Directory() { FullPath = x }));
                GlobalResources.Instance.AnalysingPaths = results.ToArray();

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
