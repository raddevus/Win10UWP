﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private RichEditBox currentRichEditBox;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            EntriesListView.Items.Add("super");
            EntriesListView.Items.Add("extra");
            EntriesListView.Items.Add("maximum");
            EntriesListView.Items.Add("advanced");
            EntriesListView.Items.Add("efficient");

            EntriesListView.Items.Add("2 super");
            EntriesListView.Items.Add("2 extra");
            EntriesListView.Items.Add("2 maximum");
            EntriesListView.Items.Add("2 advanced");
            EntriesListView.Items.Add("2 efficient");
        }

        private void CreateNewEntryButton_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pi = new PivotItem();
            var entryText = String.Format("Entry{0}",rootPivot.Items.Count+1);
            pi.Header = entryText;

            RichEditBox reb = new RichEditBox();
            reb.HorizontalAlignment = HorizontalAlignment.Stretch;
            reb.VerticalAlignment = VerticalAlignment.Stretch;

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
            currentRichEditBox = reb;
        }

        private async void SaveEntryButton_Click(object sender, RoutedEventArgs e)
        {
            JournalEntry je = new JournalEntry(currentRichEditBox);
            je.Save();
        }
    }


}
