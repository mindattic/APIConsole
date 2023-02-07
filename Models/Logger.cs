using System.Collections.Concurrent;

namespace APIConsole
{

    public class Logger
    {
        public ConcurrentBag<LogItem> Items { get; set; } = new ConcurrentBag<LogItem>();

        public Logger() { }

        public bool Message(string message)
        {
            Items.Add(new LogItem(message, LogItemType.Message));
            return true;
        }

        public bool Success(string message)
        {
            Items.Add(new LogItem(message, LogItemType.Success));
            return true;
        }

        public bool Error(string message)
        {
            Items.Add(new LogItem(message, LogItemType.Error));
            return true;
        }

        public void Clear()
        {
            Items.Clear();
        }

    }

    public enum LogItemType
    {
        Message = 0,
        Success = 1,
        Error = 2
    }

    public class LogItem
    {
        public string Message { get; set; }
        public DateTime DateAdded { get; set; }
        public LogItemType Type { get; set; }
        public LogItem(string message, LogItemType type = LogItemType.Message)
        {
            Message = message;
            Type = type;
            DateAdded = DateTime.Now;
        }
    }
}
