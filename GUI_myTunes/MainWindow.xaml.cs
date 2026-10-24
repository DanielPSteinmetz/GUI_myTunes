using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI_myTunes
{
    public partial class MainWindow : Window
    {
        private MusicRepo musicRepo;
        private DataTable songTable;

        public MainWindow()
        {
            InitializeComponent();

            musicRepo = new();

            songTable = musicRepo.SongTable;
            songsDataGrid.ItemsSource = songTable.DefaultView;

            playlistListBox.ItemsSource = musicRepo.Playlists;
        }

        // Chasen
        private void ImportMusicButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Chasen
        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Either
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Daniel
        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // Daniel
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Daniel
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}   