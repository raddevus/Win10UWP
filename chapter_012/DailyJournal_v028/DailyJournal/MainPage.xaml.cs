using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DailyJournal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private JournalEntries currentJournalEntries;

        private Windows.Storage.StorageFolder appHomeFolder;
        private String YMDDate;
        
        public MainPage()
        {
            this.InitializeComponent();
            appHomeFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            currentJournalEntries = new JournalEntries();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private async void LoadEntriesByDate()
        {
            String ymfolder = YMDDate.Substring(0,7);
            if (Directory.Exists(Path.Combine(appHomeFolder.Path, ymfolder)))
            {
                String[] allCurrentFiles = Directory.GetFiles(
                    Path.Combine(appHomeFolder.Path, ymfolder),
                    String.Format("{0}*.rtf", YMDDate), 
                    SearchOption.TopDirectoryOnly);
                if (allCurrentFiles.Length > 0)
                {
                    LoadFileFromStorage(ymfolder,
                        System.IO.Path.GetFileName(allCurrentFiles[0]), MainRichEdit);

                    currentJournalEntries.Add(
                        new JournalEntry(MainRichEdit, appHomeFolder.Path, 
                        YMDDate, "Entry1", 
                        Path.GetFileName(allCurrentFiles[0])));

                    allCurrentFiles = allCurrentFiles.Skip(1).ToArray();

                    foreach (String f in allCurrentFiles)
                    {
                        PivotItem pi = new PivotItem();
                        var entryText = String.Format("Entry{0}", rootPivot.Items.Count + 1);

                        pi.Header = entryText;

                        RichEditBox reb = new RichEditBox();
                        reb.HorizontalAlignment = HorizontalAlignment.Stretch;
                        reb.VerticalAlignment = VerticalAlignment.Stretch;

                        LoadFileFromStorage(ymfolder, System.IO.Path.GetFileName(f), reb);

                        currentJournalEntries.Add(
                            new JournalEntry(reb,
                            Path.Combine(appHomeFolder.Path),
                            YMDDate, entryText));

                        pi.Content = reb;
                        pi.Loaded += PivotItem_Loaded;
                        rootPivot.Items.Add(pi);

                        rootPivot.SelectedIndex = 0;
                    }
                }
                else
                {
                    AddDefaultEntry();
                }
            }
            else
            {
                AddDefaultEntry();
            }
        }

        private void InitializeEntriesListView()
        {
            EntriesListView.Items.Clear();
            foreach (var item in CalculateEntryCount().OrderBy(f => f.Key))
            {
                EntriesListView.Items.Add(
                    new
                    {
                        date = item.Key,
                        entryCount = item.Value
                    }
                );
            }
        }

        private Dictionary<string,int> CalculateEntryCount()
        {
            String ymfolder = YMDDate.Substring(0, 7);
            Dictionary<string, int> allEntries = new Dictionary<string, int>();

            if (Directory.Exists(Path.Combine(appHomeFolder.Path, ymfolder)))
            {
                String[] allCurrentFiles = Directory.GetFiles(
                        Path.Combine(appHomeFolder.Path, ymfolder),
                        "*.rtf",
                        SearchOption.TopDirectoryOnly);

                foreach (string f in allCurrentFiles)
                {
                    int x = 0;
                    // if the date key is already in the collection x will be > 0
                    string strippedFileName = Path.GetFileName(f).Substring(0, 10);
                    allEntries.TryGetValue(strippedFileName, out x);
                    if (x == 0)
                    {
                        allEntries.Add(strippedFileName, 1);
                    }
                    else
                    {
                        allEntries[strippedFileName] = ++x;
                    }
                }
            }
            return allEntries;
        }

        private async void LoadFileFromStorage(String ymfolder, 
            String entryFileName, 
            RichEditBox reb)
        {
            StorageFolder subStorage = await appHomeFolder.GetFolderAsync(ymfolder);
            Windows.Storage.StorageFile currentFile =
                await subStorage.GetFileAsync(entryFileName);
            Stream s = await currentFile.OpenStreamForReadAsync();
            reb.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf,
                s.AsRandomAccessStream());
            s.Dispose();
        }

        private void AddDefaultEntry()
        {
            currentJournalEntries.Add(new JournalEntry(MainRichEdit, appHomeFolder.Path, 
                YMDDate, "Entry1"));
        }

        private void CreateNewEntryButton_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pi = new PivotItem();
            var entryText = String.Format("Entry{0}",rootPivot.Items.Count+1);
            
            pi.Header = entryText;

            RichEditBox reb = new RichEditBox();
            reb.HorizontalAlignment = HorizontalAlignment.Stretch;
            reb.VerticalAlignment = VerticalAlignment.Stretch;

            currentJournalEntries.Add(
                new JournalEntry(reb, 
                Path.Combine(appHomeFolder.Path),
                YMDDate,entryText));
            
            pi.Content = reb;
            pi.Loaded += PivotItem_Loaded;
            rootPivot.Items.Add(pi);
            rootPivot.SelectedIndex = rootPivot.Items.Count - 1;
        }

        private void rootPivot_GotFocus(object sender, RoutedEventArgs e)
        {
            Pivot p = sender as Pivot;
            PivotItem pi = p.SelectedItem as PivotItem;
            RichEditBox_SetFocus(pi);
        }

        private void PivotItem_Loaded(System.Object sender, RoutedEventArgs e)
        {
            PivotItem pi = sender as PivotItem;
            RichEditBox_SetFocus(pi);
        }

        private void RichEditBox_SetFocus(PivotItem pi)
        {
            RichEditBox reb = pi.Content as RichEditBox;
            reb.Focus(FocusState.Keyboard);
            currentJournalEntries.GetSelectedEntry(pi.Header.ToString());
        }

        private async void SaveEntryButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentJournalEntries.currentJournalEntry != null)
            {
                currentJournalEntries.currentJournalEntry.Save();
            }
        }

        private void MainCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
           while(rootPivot.Items.Count > 1)
            {
                rootPivot.Items.RemoveAt(rootPivot.Items.Count - 1);
            }
            MainRichEdit.Document.SetText(TextSetOptions.ApplyRtfDocumentDefaults, "");
            currentJournalEntries.Clear();
            if (MainCalendar.SelectedDates.Count > 0)
            {
                YMDDate = MainCalendar.SelectedDates[0].ToString("yyyy-MM-dd");
                LoadEntriesByDate();
                InitializeEntriesListView();
            }
        }

        private void MainCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            MainCalendar.SelectedDates.Add(System.DateTime.Now);
        }

        private void EntriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic currentAnon = e.AddedItems[0];
            MainCalendar.SelectedDates.Clear();
            DateTime dt = DateTime.Parse(currentAnon.date);
            MainCalendar.SelectedDates.Add(dt);
        }
    }


}
