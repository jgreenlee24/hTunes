using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

/**
 * Authors: Justin Greenlee, Keith Cozad
 * Date: October 21, 2015
 * Class: Graphical User Interface
 * Assignment: hTunes
 * */

namespace hTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MusicLib musicLib;

        // Used to track initial left button click
        private Point startPoint;

        public MainWindow()
        {
            InitializeComponent();
            musicLib = new MusicLib();
            UpdateList();
            InitializeGrid();
        }

        //  Adds Playlists to ListBox1
        private void UpdateList()
        {
            listBox1.Items.Clear();
            if (!musicLib.PlaylistExists("All Music"))
            {
                listBox1.Items.Add("All Music");
                musicLib.AddPlaylist("All Music");
                foreach (string id in musicLib.SongIds)
                {
                    musicLib.AddSongToPlaylist(Int32.Parse(id), "All Music");
                }
            }

            foreach (string playlist in musicLib.Playlists)
            {
                listBox1.Items.Add(playlist);
            }

            listBox1.SelectedValue = "All Music";
        }

        // Creates "All Music" Playlist and Syncs Grid with "All Music" Playlist
        private void InitializeGrid()
        {
            DataTable table = musicLib.SongsForPlaylist("All Music");
            dataGrid.ItemsSource = table.AsDataView();
            dataGrid.IsReadOnly = false;
        }

        // Syncs Grid with Selected Playlist from ListBox1
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string playlist_name = (string)listBox1.SelectedValue;
            DataTable table = musicLib.SongsForPlaylist(playlist_name);
            dataGrid.ItemsSource = table.AsDataView();
            if (playlist_name == "All Music")
                dataGrid.IsReadOnly = false;
            else dataGrid.IsReadOnly = true;
        }

        // Displays About Dialog
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        // Prompt User for Playlist Name and Add Playlist to Library
        private void NewPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            Rename rename = new Rename("");
            rename.ShowDialog();
            if (rename.DialogResult == true)
            {
                if (!musicLib.PlaylistExists(rename.NewName))
                {
                    if (musicLib.AddPlaylist(rename.NewName))
                    {
                        musicLib.Save();
                        UpdateList();
                    }
                }
            }
        }

        // Opens File Dialog for inserting new Songs
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Song"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Media Files (.mp3, .m4a, .wma, .wav)|*.mp3; *.m4a; *.wma; *.wav"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
            }
        }

        #region Drag and drop

        private void dataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            // Start the drag-drop if mouse has moved far enough
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Initiate dragging the text from the textbox
                if (dataGrid.SelectedValue != null)
                {
                    var dataObj = new DataObject(dataGrid.SelectedValue);
                    dataObj.SetData("Row", dataGrid.SelectedValue);
                    DragDrop.DoDragDrop(dataGrid, dataObj, DragDropEffects.Copy);
                }
            }
        }

        private void dataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        private void listBox1_Drop(object sender, DragEventArgs e)
        {
            // code inspired from: http://stackoverflow.com/questions/6938752/wpf-how-do-i-handle-a-click-on-a-listbox-item
            // and: http://stackoverflow.com/questions/1719013/obtaining-dodragdrop-dragsource

            // Get the playlist dropping on
            string playlist = "";
            var item = ItemsControl.ContainerFromElement(listBox1, e.OriginalSource as DependencyObject) as ListBoxItem;
            playlist = item.Content as String;

            // Get the song from e
            var row = (e.Data.GetData("Row") as DataRowView).Row;
            int songId = Int32.Parse(row.Field<string>("id"));

            //ListBoxItem target = sender as ListBoxItem;

            // Add song to Playlist (data validation is done in DragOver event)
            musicLib.AddSongToPlaylist(songId, playlist);
            musicLib.Save();
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            // Get the playlist dropping on
            string playlist = "";
            var item = ItemsControl.ContainerFromElement(listBox1, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                playlist = item.Content as String;

                // check if dropping onto "All Music"
                if (playlist == "All Music")
                    e.Effects = DragDropEffects.None;

                // get the song id
                var row = (e.Data.GetData("Row") as DataRowView).Row;
                int songId = Int32.Parse(row.Field<string>("id"));

                // check if song is already in playlist
                var songs = musicLib.SongsForPlaylist(playlist).Select("id=" + songId);
                if (songs.Count() > 0)
                    e.Effects = DragDropEffects.None;
            }
        }

        #endregion

        #region More info

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #endregion

        #region Context menu

        // Rename Playlist
        private void Rename_MenuItemClick(object sender, RoutedEventArgs e)
        {
            string playlist_name = (string)listBox1.SelectedValue;
            if (playlist_name != "All Music")
            {
                Rename rename = new Rename(playlist_name);
                rename.ShowDialog();
                if (rename.DialogResult == true)
                {
                    if (musicLib.RenamePlaylist(playlist_name, rename.NewName))
                    {
                        musicLib.Save();
                        UpdateList();
                    }
                }
            }
            else
            {
                MessageBox.Show("You can't rename All Music");
            }
        }

        // Delete Playlist
        private void Delete_MenuItemClick(object sender, RoutedEventArgs e)
        {
            string playlist_name = (string)listBox1.SelectedValue;
            if (playlist_name != "All Music")
            {
                if (musicLib.DeletePlaylist(playlist_name))
                {
                    musicLib.Save();
                    UpdateList();
                }
            }
            else
            {
                MessageBox.Show("You can't delete All Music");
            }
        }

        #endregion
    }
}
