using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace DailyJournal
{
    public class JournalEntry
    {
        private RichEditBox _richEditBox;
        private String AppHomeFolderPath;
        private String YMFolder;
        private String YMDDate;
        private String FileName;
        public String EntryHeader { get; }

        public JournalEntry(RichEditBox richEditBox, 
            String AppHomeFolderPath,
            String YMDDate,
            String EntryHeader, 
            String FileName = null)
        {
            _richEditBox = richEditBox;
            this.AppHomeFolderPath = AppHomeFolderPath;
            this.YMDDate = YMDDate;
            this.YMFolder = YMDDate.Substring(0, 7);
            this.EntryHeader = EntryHeader;
            if (FileName == null)
            {
                GenerateFileName();
            }
            else
            {
                this.FileName = FileName;
            }
        }

        public bool Delete()
        {
            String targetFile = Path.Combine(AppHomeFolderPath, YMFolder, FileName);
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
                // We use FileName.Length - 7 because the 
                // filename has the extension on it (.rtf)
                UpdateFileNumbers(
                    Convert.ToInt16(FileName.Substring(FileName.Length - 7, 3))-1);
                return true;
            }
            return false;
        }

        private void UpdateFileNumbers(int startAtNumber)
        {
            string targetDirectory = Path.Combine(AppHomeFolderPath, YMFolder);
            if (Directory.Exists(targetDirectory))
            {
                String[] allCurrentFiles = Directory.GetFiles(targetDirectory,
                    String.Format("{0}*.rtf", YMDDate), SearchOption.TopDirectoryOnly);
                allCurrentFiles = allCurrentFiles.Skip(startAtNumber).ToArray();
                foreach (String f in allCurrentFiles)
                {
                    var targetFileName = String.Format("{0}-{1}.rtf", 
                        YMDDate, (++startAtNumber).ToString("000"));
                    File.Move(f, Path.Combine(targetDirectory,targetFileName));
                }
            }
        }

        private void GenerateFileName()
        {
            string targetDirectory = Path.Combine(AppHomeFolderPath, YMFolder);
            if (Directory.Exists(targetDirectory)){
                String[] allCurrentFiles = Directory.GetFiles(targetDirectory,
                    String.Format("{0}*.rtf", YMDDate), SearchOption.TopDirectoryOnly);
                if (allCurrentFiles.Length <= 0)
                {
                    FileName = String.Format("{0}-{1}.rtf", YMDDate, 1.ToString("000"));
                }
                else
                {
                    List<short> fileNumbers = new List<short>();
                    foreach (string f in allCurrentFiles)
                    {
                        String fileName = System.IO.Path.GetFileNameWithoutExtension(f);
                        fileNumbers.Add(Convert.ToInt16(fileName.Substring(fileName.Length - 3, 3)));
                    }
                    int maxValue = fileNumbers.Max<short>();
                    maxValue++;
                    FileName = String.Format("{0}-{1}.rtf", YMDDate, maxValue.ToString("000"));
                }
            }
            else
            {
                FileName = String.Format("{0}-{1}.rtf", YMDDate, 1.ToString("000"));
            }
        }

        public async Task Save()
        {
            StorageFolder storageFolder = 
                Windows.Storage.ApplicationData.Current.LocalFolder;
            if (!Directory.Exists(Path.Combine(AppHomeFolderPath,YMFolder)))
            {
                await storageFolder.CreateFolderAsync(YMFolder);
            }
           
           StorageFolder subStorage = await storageFolder.GetFolderAsync(YMFolder);
           StorageFile sampleFile =
                await subStorage.CreateFileAsync(
                    FileName,
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream documentStream =
                await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            _richEditBox.Document.SaveToStream(TextGetOptions.FormatRtf, documentStream);
            documentStream.Dispose();
        }
    }
}
