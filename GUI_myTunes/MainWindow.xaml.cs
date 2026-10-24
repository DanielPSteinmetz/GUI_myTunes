using MiniPlayerWpf;
using System.Data;
using System.Numerics;
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
        private MusicRepo musicRepo = new();
        private MusicPlayer musicPlayer = new();

        public MainWindow()
        {
            InitializeComponent();

            songsDataGrid.ItemsSource = musicRepo.SongTable.DefaultView;
            playlistListBox.ItemsSource = musicRepo.Playlists;

            musicPlayer.StateChanged += (s, e) => CommandManager.InvalidateRequerySuggested();
        }

        // Chasen
        private void ImportMusicButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Dont allow creating "All Music"
        // Chasen
        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Either
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistListBox.SelectedItem is string selectedPlaylist)
            {
                if (selectedPlaylist == "All Music")
                {
                    songsDataGrid.ItemsSource = musicRepo.SongTable.DefaultView;
                    return;
                }

                DataTable playlistSongs = musicRepo.SongsForPlaylist(selectedPlaylist);
                songsDataGrid.ItemsSource = playlistSongs.DefaultView;
            }
        }

        private void songsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (songsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string filename = Convert.ToString(selectedRow["filename"]) ?? "";
                musicPlayer.Open(filename);
            }       
        }

        private void PlayCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            musicPlayer.Play();
        }

        private void PlayCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = musicPlayer.CanPlay;
            playButton.ToolTip = musicPlayer.PlayDisabledReason;
        }

        private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            musicPlayer.Stop();
        }

        private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = musicPlayer.CanStop;
            stopButton.ToolTip = musicPlayer.StopDisabledReason;
        }
    }
}   