using MiniPlayerWpf;
using System.Data;
using System.Drawing;
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
using System.Xml.Linq;
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

            playlistListBox.SelectedIndex = 0;
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
                return;
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

        // Chasen
        private void RemoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (songsDataGrid.SelectedItem is not DataRowView row) return;

            object selectedPlaylist = playlistListBox.SelectedItem;

            if (selectedPlaylist == null) return;

            String? playlistName = selectedPlaylist.ToString();
            int position = songsDataGrid.SelectedIndex;
            int id = row.Row.Field<int>("id");
            String? songName = row.Row.Field<String>("title");

            if (playlistName == "All Music")
            {
                String message = "Are you sure you want to delete " + songName + " from " + playlistName + "?";
                String title = "Confirm Delete";

                MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    musicRepo.DeleteSong(id);
                    UpdateMusicTable(musicRepo.SongTable);
                    musicRepo.Save();
                }
            }
            else if (playlistName != null)
            {
                musicRepo.RemoveSongFromPlaylist(position, id, playlistName);
                UpdateMusicTable(musicRepo.SongsForPlaylist(playlistName));
                musicRepo.Save();
            }
        }

        // Chasen
        // Song can only be removed if selected by datagrid
        private void RemoveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = songsDataGrid.SelectedItem != null;
        }

        // Dont allow creating "All Music"
        // Chasen
        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            RenameDialog nameDialog = new RenameDialog();

            if (nameDialog.ShowDialog() == true)
            {
                string name = nameDialog.playlistName;

                if (musicRepo.AddPlaylist(name))
                {
                    playlistListBox.ItemsSource = musicRepo.Playlists;
                    musicRepo.Save();
                }
                else
                {
                    MessageBox.Show("Playlist names must be unique.");
                }
            }

        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
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
            musicPlayer.Close();

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
            if (songsDataGrid.SelectedItem is not DataRowView row) return;

            int id = row.Row.Field<int>("id");

            if (musicRepo.GetSong(id) is { } song)
            {
                musicPlayer.Open(song.Filename);
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

        private void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            RenameDialog renameDialog = new RenameDialog();
            if (renameDialog.ShowDialog() == true)
            {
                string? oldName = playlistListBox.SelectedItem.ToString();
                string newName = renameDialog.playlistName;

                if (oldName != null && musicRepo.RenamePlaylist(oldName, newName))
                {
                    playlistListBox.ItemsSource = musicRepo.Playlists;
                    musicRepo.Save();
                }
                else
                {
                    MessageBox.Show("Playlist names must be unique.");
                }
            }
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            String? playlistName = playlistListBox.SelectedItem.ToString();

            if (playlistName == "All Music")
            {
                MessageBox.Show("You can't delete " + playlistName, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            String message = "Are you sure you want to delete " + playlistName + "?";
            String title = "Confirm Delete";

            MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (playlistName != null && musicRepo.DeletePlaylist(playlistName))
                {
                    playlistListBox.ItemsSource = musicRepo.Playlists;
                    musicRepo.Save();
                }
            }
        }
    }
}