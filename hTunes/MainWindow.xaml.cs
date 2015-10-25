using System;
using System.Collections.Generic;
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
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }
}
