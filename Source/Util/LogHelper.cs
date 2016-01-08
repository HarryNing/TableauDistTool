using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableauDistTool.Util
{
    public static class LogHelper
    {
        public static void Write(string message)
        {
            FileSystemLog.Instance.Write(message);
        }

        public static void Write(string format, params object[] args)
        {
            FileSystemLog.Instance.Write(String.Format(format, args));
        }
    }

    public interface ILog
    {
        void Write(string message);
    }

    public class FileSystemLog : ILog
    {
        static FileSystemLog()
        {
            Instance = new FileSystemLog(@"log.txt");
        }

        public static FileSystemLog Instance { get; private set; }

        private string _LogPath;
        private FileSystemLog(string logPath)
        {
            _LogPath = logPath;
        }

        #region ILog Members

        public void Write(string message)
        {
            message = StringHelper.GetDateString() + ": " + message + Environment.NewLine;
            System.IO.File.AppendAllText(_LogPath, message);
        }

        #endregion
    }
}
