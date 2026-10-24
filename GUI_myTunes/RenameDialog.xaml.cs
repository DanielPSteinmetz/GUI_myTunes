using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI_myTunes
{
    /// <summary>
    /// Interaction logic for PlaylistName.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {

        public string playlistName = "";
        public RenameDialog(String currentName = "")
        {
            InitializeComponent();

            playlistName = currentName;

            textBox.Text = playlistName;
        }


        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            playlistName = textBox.Text;
            DialogResult = true;
            Close();
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (textBox.Text != "" && textBox.Text != "All Music");
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
