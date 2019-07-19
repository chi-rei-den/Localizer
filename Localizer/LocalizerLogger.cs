using System;
using System.IO;

namespace Localizer
{
    /// <summary>
    /// Another logger for special use like logging the changes of texts.
    /// </summary>
    public class LocalizerLogger
    {
        private readonly string logPath;
        private readonly string textUpdateLogPath;

        public LocalizerLogger(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.logPath = Path.Combine(path, "Logs.txt");
            this.textUpdateLogPath = Path.Combine(path, "TextUpdateLogs.txt");

            File.AppendAllText(this.logPath, $"{DateTime.Now.ToString()}\n");
            File.AppendAllText(this.textUpdateLogPath, $"{DateTime.Now.ToString()}\n");
        }

        public void Log(object content)
        {
            this.Log(content.ToString());
        }

        public void Log(string content)
        {
            File.AppendAllText(this.logPath, $"{content}\n");
        }

        public void DebugLog(object content)
        {
#if DEBUG
            this.Log(content);
#endif
        }

        public void TextUpdateLog(object content)
        {
            File.AppendAllText(this.textUpdateLogPath, $"{content}\n");
        }
    }
}