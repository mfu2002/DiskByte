using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiskByte.ViewModel
{
    public class NotificationSystem : INotifyPropertyChanged
    {
        #region Private variables

        /// <summary>
        /// Lock to ensure no two threads are broadcasting a notification at the same time. 
        /// </summary>
        private readonly object _notificationLock = new object();

        #region Property Backend Variables

        /// <summary>
        /// Backend for the Instance property lazily initialised. 
        /// <seealso cref="Instance"/>
        /// </summary>
        private static readonly Lazy<NotificationSystem> _instance = new Lazy<NotificationSystem>(() => new NotificationSystem());
        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static NotificationSystem Instance { get { return _instance.Value; } }

        /// <summary>
        /// String that holds the last notification. 
        /// </summary>
        public string NotificationString { get; private set; } = "Welcome.";

        #endregion

        #region Constructor
        /// <summary>
        /// Private constructor to ensure NotificationSystem cannot be created from outside. 
        /// </summary>
        private NotificationSystem()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Pushes the Notification through. 
        /// </summary>
        /// <param name="notification"></param>
        public void PushNotification(string notification)
        {
            lock (_notificationLock)
            {
                NotificationString = notification;
                OnPropertyChanged(nameof(NotificationString));
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