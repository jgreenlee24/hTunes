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

        public MainWindow()
        {
            InitializeComponent();
            musicLib = new MusicLib();
            InitializeList();
            InitializeGrid();
        }

        //  Adds Playlists to ListBox1
        private void InitializeList(){
            listBox1.Items.Add("All Music");
            foreach (string playlist in musicLib.Playlists)
            {
                listBox1.Items.Add(playlist);
            }

            listBox1.SelectedValue = "All Music";
        }

        // Creates "All Music" Playlist and Syncs Grid with "All Music" Playlist
        private void InitializeGrid()
        {
            musicLib.AddPlaylist("All Music");
            foreach (string id in musicLib.SongIds)
            {
                musicLib.AddSongToPlaylist(Int32.Parse(id), "All Music");
            }
            DataTable table = musicLib.SongsForPlaylist("All Music");
            dataGrid.ItemsSource = table.AsDataView();
        }

        // Syncs Grid with Selected Playlist from ListBox1
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string playlist_name = (string)listBox1.SelectedValue;
            DataTable table = musicLib.SongsForPlaylist(playlist_name);
            dataGrid.ItemsSource = table.AsDataView();
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
            

        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public struct DataGridItem
    {
        public int id { get; set;}
        public string title { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string genre { get; set; }
    }
}
