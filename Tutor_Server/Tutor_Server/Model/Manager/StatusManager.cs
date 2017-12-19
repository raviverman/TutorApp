using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tutor_Server.Model.Data;

namespace Tutor_Server.Model.Manager
{
    public enum StatusType
    {
        Normal,
        Success,
        Error
    }
    public class StatusManager
    {
        private static ObservableCollection<Status> messageList = new ObservableCollection<Status>();
        public static int Count { get; private set; } = 0;
        private static string messageOpLock = "messageListOperation";

        public static Color GetColor(StatusType type)
        {
            switch(type)
            {
                case StatusType.Normal:
                    return Colors.Gray;
                case StatusType.Success:
                    return Colors.Green;
                case StatusType.Error:
                    return Colors.Red;
                default:
                    return Colors.Gray;
            }
        }

        public static void PushMessage(string Message, StatusType type = StatusType.Normal)
        {
            lock (messageOpLock)
            {
                messageList.Add(new Status { StatusMessage=Message, Type = type });
                Count = messageList.Count;
            }
        }

        public static void PullMessages(ObservableCollection<Status> Result)
        {
            lock (messageOpLock)
            {
                while(messageList.Count!=0)
                {
                    Result.Add(messageList.ElementAt(0));
                    messageList.RemoveAt(0);
                    Count = messageList.Count;
                }
            }
        }
                    
    }
}
