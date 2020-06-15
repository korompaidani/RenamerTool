using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string workingPath = @"C:\Users\korom\Desktop\Canon 450D";

            var logger = new Logger(workingPath);
            var fileReader = new FileReader(workingPath, logger);

            try
            {
                fileReader.ReadAllFiles();
            }
            finally
            {
                logger.WriteLogToFile();
            }
        }
    }
}
