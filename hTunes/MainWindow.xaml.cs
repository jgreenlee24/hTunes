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
        }

        private void InitializeList(){
            listBox1.Items.Add("All Music");
            foreach (string playlist in musicLib.Playlists)
            {
                listBox1.Items.Add(playlist);
            }
            listBox1.SelectedValue = "All Music";
            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            string playlist_name = (string)listBox1.SelectedValue;
            if (playlist_name == "All Music")
            {
                foreach (string id in musicLib.SongIds)
                {
                    Song song = musicLib.GetSong(Int32.Parse(id));
                    DataGridItem item = new DataGridItem();
                    item.id = song.Id;
                    item.title = song.Title;
                    item.artist = song.Artist;
                    item.album = song.Album;
                    item.genre = song.Genre;
                    dataGrid.Items.Add(item);
                }
            }
            else
            {
                dataGrid.ItemsSource = musicLib.SongsForPlaylist(playlist_name).AsDataView();
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
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
