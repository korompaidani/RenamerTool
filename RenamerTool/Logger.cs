using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerTool
{
    public class Logger
    {
        private const string LogFileConstTag = "log_";
        private const string LogFileExtension = ".txt";
        private string logFileName;
        private string logFilePath;
        private List<string> logDb;

        public Logger(string path)
        {
            logFilePath = path;
            logFileName = LogFileConstTag + DateTime.Now.ToString("yyyyMMddHHmmss") + LogFileExtension;
            logDb = new List<string>();
        }

        public void AddLogEntry(string entry)
        {
            logDb.Add(entry);
        }

        public void WriteLogToFile()
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(logFilePath, logFileName)))
            {
                foreach (string line in logDb)
                    outputFile.WriteLine(line);
            }
        }
    }
}
