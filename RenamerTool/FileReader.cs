using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RenamerTool
{
    public class FileReader
    {
        private const string ImageTag = "SAN_";

        private DirectoryInfo DirectoryPath;
        private Dictionary<string, string> imageFiles;
        private Logger logger;

        public FileReader(string directoryPath, Logger logger)
        {
            DirectoryPath = new DirectoryInfo(directoryPath);
            imageFiles = new Dictionary<string, string>();
            this.logger = logger;
        }

        public void ReadAllFiles()
        {
            foreach(var file in DirectoryPath.GetFiles("*.jpg"))
            {
                try
                {
                    ProcessImageFile(file);
                }
                catch(Exception ex)
                {
                    CreateMessage($"!!ERROR related to {file.Name}: {ex.Message}");
                }
            }

            foreach (var file in DirectoryPath.GetFiles("*.JPEG"))
            {
                try
                {
                    ProcessImageFile(file);
                }
                catch (Exception ex)
                {
                    CreateMessage($"!!ERROR related to {file.Name}: {ex.Message}");
                }
            }

            foreach (var file in DirectoryPath.GetFiles("*.mov"))
            {
                ProcessVideoFile(file);
            }
        }

        private void ProcessVideoFile(FileInfo filePath)
        {
            var oldFileName = Path.GetFileNameWithoutExtension(filePath.Name);
            if (imageFiles.ContainsKey(oldFileName))
            {            
                RenameFile(oldFileName, imageFiles[oldFileName], filePath);
            }
        }

        private void ProcessImageFile(FileInfo filePath)
        {
            string dateTaken = string.Empty;
            string cameraModel = null;
            string returnDateTaken = null;

            using (var fs = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource bitSource = BitmapFrame.Create(fs);
                BitmapMetadata metaData = (BitmapMetadata)bitSource.Metadata;
                cameraModel = metaData.CameraModel;
                returnDateTaken = metaData.DateTaken;
                if (returnDateTaken != null)
                {                    
                    dateTaken = DateTime.Parse(returnDateTaken).ToString("yyyyMMdd_HHmmss");
                }                
            }

            if (cameraModel == null || returnDateTaken == null)
            {                
                CreateMessage("!NO METADATA TO " + filePath.Name);
                return;
            }

            var oldFileName = Path.GetFileNameWithoutExtension(filePath.Name);
            var newFileName = CreateCustomFileName(oldFileName, dateTaken, cameraModel);
            imageFiles.Add(oldFileName, newFileName);
            RenameFile(oldFileName, newFileName, filePath);

        }

        private string CreateCustomFileName(string originalFileName, string formattedTakenDate, string cameraModel)
        {
            var underScoreCameraModel = cameraModel.Replace(' ', '_');
            return ImageTag + formattedTakenDate + '_' + underScoreCameraModel + '_' + originalFileName;
        }

        private void RenameFile(string oldName, string newName, FileInfo fileInfo)
        {
            var sourceFileName = fileInfo.FullName;
            var destinationFileName = Path.Combine(fileInfo.DirectoryName, newName) + fileInfo.Extension;
            File.Move(sourceFileName, destinationFileName);

            CreateMessage($"{oldName} renamed to {newName}{fileInfo.Extension} was successfull.");
        }

        private void CreateMessage(string message)
        {
            Console.WriteLine(message);
            logger.AddLogEntry(message);
        }
    }
}
