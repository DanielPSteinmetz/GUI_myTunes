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
using TagLib;
using TagLib.Jpeg;

using Point = System.Windows.Point;
using Vector = System.Windows.Vector;
using TextBox = System.Windows.Controls.TextBox;
using Microsoft.Win32;

namespace GUI_myTunes
{
    public partial class MainWindow : Window
    {
        private MusicRepo musicRepo = new();
        private MusicPlayer musicPlayer = new();
        private DataTable currentMusicTable;
        private string filter = "";
        private Point startPos;
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


        // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/how-to-open-common-system-dialog-box
        private void ImportMusicButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".mp3";

            // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.filedialog.filter?view=windowsdesktop-10.0
            dialog.Filter = "Audio Files (*.mp3;*.m4a;*.wma;*.wav)|*.mp3;*.m4a;*.wma;*.wav|All Files (*.*)|*.*";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                Song newSong = musicRepo.AddSong(filename);
                int newSongId = newSong.Id;

                musicRepo.Save();
                UpdateMusicTable(musicRepo.SongTable);

                // we show the new "All Music" playlist, so select it as well
                playlistListBox.SelectedIndex = 0;

                foreach (var item in songsDataGrid.Items)
                {
                    if (item is DataRowView row && row.Row.Field<int>("id") == newSongId)
                    {
                        songsDataGrid.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void RemoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (songsDataGrid.SelectedItem is not DataRowView row) return;

            object selectedPlaylist = playlistListBox.SelectedItem;

            if (selectedPlaylist == null) return;

            String? playlistName = selectedPlaylist.ToString();
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
                int position = row.Row.Field<int>("position");
                musicRepo.RemoveSongFromPlaylist(position, id, playlistName);
                UpdateMusicTable(musicRepo.SongsForPlaylist(playlistName));
                musicRepo.Save();
            }
        }

        private void RemoveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = songsDataGrid.SelectedItem != null;
        }

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            RenameDialog nameDialog = new RenameDialog();
            nameDialog.Owner = this;

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

        private void songsDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(null);
            Vector diff = startPos - currentPoint;

            if (e.LeftButton == MouseButtonState.Pressed &&
                songsDataGrid.SelectedItem is DataRowView row &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                int id = row.Row.Field<int>("id");
                DragDrop.DoDragDrop(songsDataGrid, new DataObject("songId", id), DragDropEffects.Copy);
            }
        }

        private void songsDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPos = e.GetPosition(null);
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


        private void RenameCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RenameDialog renameDialog = new RenameDialog();
            renameDialog.Owner = this;
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

        private void RenameCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = playlistListBox.SelectedItem != null && playlistListBox.SelectedItem.ToString() != "All Music";
        }


        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            String? playlistName = playlistListBox.SelectedItem.ToString();

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

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = playlistListBox.SelectedItem != null && playlistListBox.SelectedItem.ToString() != "All Music";
        }

        // asked ai how to find what playlist was dropped into
        private T? FindParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = VisualTreeHelper.GetParent(child);
            }

            return null;
        }

        private void playlistListBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("songId"))
            {
                return;
            }

            if (e.Data.GetData("songId") is not int songId)
            {
                return;
            }

            if (e.OriginalSource is not DependencyObject source)
            {
                return;
            }


            ListBoxItem? item = FindParent<ListBoxItem>(source);

            if (item == null)
            {
                return;
            }

            string? playlistName = item.Content.ToString();

            if (playlistName == "All Music" || playlistName == null)
            {
                return;
            }

            musicRepo.AddSongToPlaylist(songId, playlistName);
            musicRepo.Save();
            
            if (playlistListBox.SelectedItem?.ToString() == playlistName)
            {
                UpdateMusicTable(musicRepo.SongsForPlaylist(playlistName));
            }
        }

        private void playlistListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("songId"))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            musicRepo.Save();
        }


    }
}