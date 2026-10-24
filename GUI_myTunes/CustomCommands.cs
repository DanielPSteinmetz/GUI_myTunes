using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GUI_myTunes
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Remove = new RoutedUICommand(
            "Remove", "Remove", typeof(CustomCommands),
            new InputGestureCollection());
    }

}
