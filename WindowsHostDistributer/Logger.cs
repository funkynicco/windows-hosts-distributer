using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer
{
    public enum LogType
    {
        Normal,
        Debug,
        Error,
        Warning
    }

    public static class Logger
    {
        public class Message
        {
            public LogType Type { get; private set; }
            public DateTime UtcDate { get; private set; }
            public string Text { get; private set; }

            public Message(LogType type, DateTime utcDate, string text)
            {
                Type = type;
                UtcDate = utcDate;
                Text = text;
            }

            public Message(LogType type, string text) :
                this(type, DateTime.UtcNow, text)
            {
            }
        }

        private static LinkedList<Message> _messages = new LinkedList<Message>();
        private static readonly object _lock = new object();

        public static void Log(LogType type, string message)
        {
            var obj = new Message(type, message);
            lock (_lock)
                _messages.AddLast(obj);
        }

        public static void Log(LogType type, string format, params object[] args)
        {
            Log(type, string.Format(format, args));
        }

        public static void ProcessMessage(Action<Message> messageCallback)
        {
            LinkedList<Message> oldMessages;
            lock (_lock)
            {
                oldMessages = _messages;
                _messages = new LinkedList<Message>();
            }

            foreach (var msg in oldMessages)
                messageCallback(msg);
        }
    }
}
