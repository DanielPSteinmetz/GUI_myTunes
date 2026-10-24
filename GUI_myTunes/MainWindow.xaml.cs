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
using TagLib.Jpeg;

namespace GUI_myTunes
{
    public partial class MainWindow : Window
    {
        private MusicRepo musicRepo = new();
        private MusicPlayer musicPlayer = new();
        private DataTable currentMusicTable;
        private string filter = ""; 

        public MainWindow()
        {
            InitializeComponent();

            currentMusicTable = musicRepo.SongTable;
            UpdateMusicTable(musicRepo.SongTable);

            playlistListBox.ItemsSource = musicRepo.Playlists;

            musicPlayer.StateChanged += (s, e) => CommandManager.InvalidateRequerySuggested();
        }

        // ai assisted
        private void UpdateMusicTable(DataTable newTable)
        {
            currentMusicTable = newTable;

            DataView view = newTable.DefaultView;

            if (string.IsNullOrWhiteSpace(filter))
            {
                view.RowFilter = "";
                songsDataGrid.ItemsSource = view;
            }

            string escaped = EscapeLike(filter.Trim());

            view.RowFilter =
                $"title LIKE '%{escaped}%' OR " +
                $"artist LIKE '%{escaped}%' OR " +
                $"album LIKE '%{escaped}%' OR " +
                $"genre LIKE '%{escaped}%'";

            songsDataGrid.ItemsSource = view;
        }

        private string EscapeLike(string value)
        {
            var sb = new StringBuilder();
            foreach (char c in value)
            {
                switch (c)
                {
                    case '\'':
                        sb.Append("''");
                        break;
                    case '*':
                    case '%':
                    case '[':
                    case ']':
                        sb.Append('[').Append(c).Append(']');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
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

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                filter = textBox.Text;
                UpdateMusicTable(currentMusicTable);
            }
        }

        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistListBox.SelectedItem is string selectedPlaylist)
            {
                if (selectedPlaylist == "All Music")
                {
                    UpdateMusicTable(musicRepo.SongTable);
                    return;
                }

                UpdateMusicTable(musicRepo.SongsForPlaylist(selectedPlaylist));
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