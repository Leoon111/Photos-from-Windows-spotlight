using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        private FolderBrowserDialog _folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

        private String _pathSaveImages;

        private StartProgram _startProgram;

        // todo необходим рефракторинг

        public MainWindow()
        {
            InitializeComponent();
            // Этот метод будет запускать фоновые задачи, сейчас не реализую
            _startProgram = new StartProgram();
            _startProgram.Run(this);
        }

        private void ButtonPathToDirectory_Click(object sender, RoutedEventArgs e)
        {

            // выбрать место сохнанения.

            _folderBrowserDialog.Description = "Выберите папку, куда будут сохраняться картинки";
            /// Тестовая папка на моем компе, проверяю, если ее нет, то открываем Мой Компьютер (если не мой компьютер)
            _folderBrowserDialog.SelectedPath =
                Directory.Exists(@"E:\OneDrive\Новые фотографии\1\test\1\")
                ? @"E:\OneDrive\Новые фотографии\1\test\1\"
                : Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            /// метод сохранения
            if (_folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxPathToDirectory.Text = _pathSaveImages = _folderBrowserDialog.SelectedPath;

                _startProgram.SetTextOutputForWin($"Картинки сохраняем по адресу:\n {_pathSaveImages}");

                _startProgram.SetTextOutputForWin("Найдены картинки с датами:");
                /// переменная для вывода дат найденных картинок
                var DateOfTheImagesFound = new List<DateTime>();

                var pathGoodPhotos = _startProgram.SearchFilesInWindowsFolder();

                /// Перебираем адреса найденных картинок, создавая колекцию дат картинок для информации.
                foreach (var pathGoodPhoto in pathGoodPhotos)
                {
                    DateOfTheImagesFound.Add(File.GetCreationTime(pathGoodPhoto));
                }

                /// Сортируем коллекцию по умолчанию
                DateOfTheImagesFound.Sort();

                /// Выводим данные на консоль.
                foreach (var item in DateOfTheImagesFound)
                {
                    _startProgram.SetTextOutputForWin(item.ToString());
                }
                Console.WriteLine("\n");

                /// скопировать и переименовать файлы
                _startProgram.SaveMethod(pathGoodPhotos, _folderBrowserDialog);
            }

            /// Сообщение, если не выбрали папку для расположения файлов
            else
            {
                _startProgram.SetTextOutputForWin("Копирование не выполнено по причине невыбранной папки назначения");
            }
            _folderBrowserDialog.Dispose();

        }
    }
}

