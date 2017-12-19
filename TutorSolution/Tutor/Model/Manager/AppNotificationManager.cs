using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Model.Data;

namespace Tutor.Model.Manager
{
    public class AppNotificationManager
    {
        private static ObservableCollection<AppNotification> messageList = new ObservableCollection<AppNotification>();
        public static int Count { get; private set; } = 0;
        private static string messageOpLock = "messageListOperation";

        public static void PushMessage(AppNotification notice)
        {
            lock (messageOpLock)
            {
                messageList.Add(notice);
                Count = messageList.Count;
            }
        }

        public static AppNotification PullMessage()
        {
            lock (messageOpLock)
            {
                AppNotification notice = messageList.ElementAt(0);
                messageList.RemoveAt(0);
                Count = messageList.Count;
                return notice;
            }
        }

    }
}
