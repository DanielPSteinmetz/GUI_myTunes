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

        string playlistName = "";
        public RenameDialog(String currentName = "")
        {
            InitializeComponent();

            playlistName = currentName;

            textBox.Text = playlistName;
        }



        private void SaveCommand_Executed(object sender, RoutedEventArgs e)
        {
            playlistName = textBox.Text;
            Close();
        }
        private void CloseCommand_Executed(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
