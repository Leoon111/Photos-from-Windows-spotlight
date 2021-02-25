using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace PhotoFromScreensaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnFolderPathValidationError(object? sender, ValidationErrorEventArgs e)
        {
            var control = (Control) e.OriginalSource;
            if (e.Action == ValidationErrorEventAction.Added)
                control.ToolTip = e.Error.ErrorContent.ToString();
            else
                control.ClearValue(ToolTipProperty);
        }
    }
}
