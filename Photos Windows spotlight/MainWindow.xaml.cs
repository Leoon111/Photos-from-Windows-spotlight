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

namespace Photos_Windows_spotlight
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Этот метод будет запускать фоновые задачи, сейчас не реализую
            StartProgram.Run();
        }

        private void ButtonPathToDirectory_Click(object sender, RoutedEventArgs e)
        {
            

            //// выбрать место сохнанения.
            //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            //folderBrowserDialog.Description = "Выберите папку, куда будут сохраняться картинки";
            ///// Тестовая папка на моем компе, проверяю, если ее нет, то открываем Мой Компьютер (если не мой компьютер)
            //folderBrowserDialog.SelectedPath =
            //    Directory.Exists(@"E:\OneDrive\Новые фотографии\1\test\1\")
            //    ? @"E:\OneDrive\Новые фотографии\1\test\1\"
            //    : Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        }
    }
}
