using System;
using System.IO;
using System.Text;
using Terraria;

namespace Localizer
{
    /// <summary>
    /// Another logger for special use like logging the changes of texts.
    /// </summary>
    public class LocalizerLogger
    {
        private string logPath;
        private string textUpdateLogPath;

        public LocalizerLogger(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            logPath = Path.Combine(path, "Logs.txt");
            textUpdateLogPath = Path.Combine(path, "TextUpdateLogs.txt");
            
            File.AppendAllText(logPath, $"{DateTime.Now.ToString()}\n");
            File.AppendAllText(textUpdateLogPath, $"{DateTime.Now.ToString()}\n");
        }
        
        public void Log(object content)
        {
            Log(content.ToString());
        }

        public void Log(string content)
        {
            File.AppendAllText(logPath, $"{content}\n");
        }

        public void DebugLog(object content)
        {
#if DEBUG
            Log(content);
#endif
        }

        public void TextUpdateLog(object content)
        {
            File.AppendAllText(textUpdateLogPath, $"{content}\n");
        }
    }
}