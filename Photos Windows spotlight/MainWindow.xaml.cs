using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using ImagesWindowsSpotlight.lib.Models;
using ImagesWindowsSpotlight.lib.Service;

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

        private List<PHashAndDataImage> _goodPhotosCollectionsPath;

        private XMLData _xMLData;

        // todo необходим рефракторинг

        public MainWindow()
        {
            InitializeComponent();
            // Этот метод будет запускать фоновые задачи, сейчас не реализую
            _startProgram = new StartProgram();
            _xMLData = _startProgram.Run(this);
            // полсе работы метода проверяем, если мы внесли изменения в текстбокс, то здесь это изменение присваиваем переменной
            // ! на данный момент так, покак не решил как буду делать по другому
            if (TextBoxPathToDirectory.Text != "")
            {
                _pathSaveImages = TextBoxPathToDirectory.Text;
            }
        }

        private void ButtonPathToDirectory_Click(object sender, RoutedEventArgs e)
        {

            // выбрать место сохнанения.

            _folderBrowserDialog.Description = "Выберите папку, куда будут сохраняться картинки";
            /// Тестовая папка на моем компе, проверяю, если ее нет, то открываем Мой Компьютер (если не мой компьютер)
            _folderBrowserDialog.SelectedPath =
                Directory.Exists(@"D:\OneDrive\Новые фотографии\1\test\1\")
                ? @"D:\OneDrive\Новые фотографии\1\test\1\"
                : Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            /// метод сохранения
            if (_folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxPathToDirectory.Text = _pathSaveImages = _folderBrowserDialog.SelectedPath;

                // сразу сохраняем место сохранения в конфигурационный файл (на данный момент так)
                _xMLData.AddNewConfiguration(new Configuration() {PathFiles = _pathSaveImages});

                _startProgram.SetTextOutputForWin($"Картинки сохраняем по адресу:\n {_pathSaveImages}");

                SaveImagesButton.IsEnabled = ValidateVisibleSaveButton();

            }

            /// Сообщение, если не выбрали папку для расположения файлов
            else
            {
                _startProgram.SetTextOutputForWin("Необходимо выбрать папку для сохранения файлов");
            }
            _folderBrowserDialog.Dispose();

        }

        private void FindNewImagesButton_Click(object sender, RoutedEventArgs e)
        {
            // переменная для вывода дат найденных картинок
            var dateOfTheImagesFound = new List<DateTime>();

            //_goodPhotosCollectionsPath = _startProgram.SearchFilesInWindowsFolder();
            _goodPhotosCollectionsPath =
                new ImageService().SearchImagesInFolder(
                    @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");

            // Перебираем адреса найденных картинок, создавая колекцию дат картинок для информации.
            //foreach (var pathGoodPhoto in _goodPhotosCollectionsPath)
            //{
            //    dateOfTheImagesFound.Add(File.GetCreationTime(pathGoodPhoto));
            //}

            // Сортируем коллекцию по умолчанию
            //dateOfTheImagesFound.Sort();

            // Формируем сообщение для вывода в окно.
            string outputMessage = "Найдены картинки с датами загрузки в Windows:";
            //foreach (var item in dateOfTheImagesFound)
            //{
            //    outputMessage += $"\n{dateOfTheImagesFound.IndexOf(item)} - {item.ToString()}";
            //}

            _startProgram.SetTextOutputForWin(outputMessage);
            SaveImagesButton.IsEnabled = ValidateVisibleSaveButton();
        }

        private void SaveImagesButton_Click(object sender, RoutedEventArgs e)
        {
            /// скопировать и переименовать файлы
            //_startProgram.SaveMethod(_goodPhotosCollectionsPath, _pathSaveImages);
        }

        /// <summary>
        /// Проверка, должна ли кнопка Сохранить быть активна.
        /// </summary>
        /// <returns></returns>
        private bool ValidateVisibleSaveButton()
        {
            return _goodPhotosCollectionsPath != null && _goodPhotosCollectionsPath.Count > 0 && _pathSaveImages != null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

