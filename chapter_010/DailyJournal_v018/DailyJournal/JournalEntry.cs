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
        public String EntryHeader { get; }

        public JournalEntry(RichEditBox richEditBox, String AppHomeFolderPath,
            String YMFolder,
            String EntryHeader)
        {
            _richEditBox = richEditBox;
            this.AppHomeFolderPath = AppHomeFolderPath;
            this.YMFolder = YMFolder;
            this.EntryHeader = EntryHeader;
        }

        public async void Save()
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
                    "FirstRichEdit.rtf",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream documentStream =
                await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            _richEditBox.Document.SaveToStream(TextGetOptions.FormatRtf, documentStream);
            documentStream.Dispose();
        }
    }
}
