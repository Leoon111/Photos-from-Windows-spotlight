using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using ImagesWindowsSpotlight.lib;
using ImagesWindowsSpotlight.lib.Service;

namespace Photos_Windows_spotlight
{
    class StartProgram
    {
        private MainWindow _mainWindow;
        private XMLData _xmlData;
        private Configuration _configuration;
        private readonly string _pathToPicturesLocal;
        private ImageService _imageService;
        public StartProgram()
        {
            _xmlData = new XMLData();
            // Запускать проверку в фоне при старет


            // путь к файлу где в Виндовс находятся картинки для заставки
            _pathToPicturesLocal = @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";

            _imageService = new ImageService();
        }

        /// <summary>
        /// Запускает автоматические действия проверки файла конфигурации.
        /// </summary>
        /// <param name="mainWindow"></param>
        public XMLData Run(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            SetTextOutputForWin("! Внимание ! В данный момент все данные выводятся в данное окно.\n");
            try
            {
                // проверяем, есть ли в дирректроии файл с конфигурацией
                if (XMLData.IsExistFileConfiguration())
                {
                    SetTextOutputForWin("Файл конфигурации найден.");
                    _configuration = _xmlData.GetXmlConfigurations();
                    if (_configuration != null)
                    {
                        _mainWindow.TextBoxPathToDirectory.Text = _configuration.PathFiles;
                    }
                    else
                    {
                        SetTextOutputForWin("! Файл конфигурации пуст.");
                    }
                }
                else
                {
                    SetTextOutputForWin("Файл конфигурайии не найден.");
                    XMLData.CreateXMLFile();
                    SetTextOutputForWin("Создан новый файл конфигурации.");
                }
            }
            catch (Exception ex)
            {
                // Подкрасить красным, пока вывожу все данные в окно.
                SetTextOutputForWin($"! Произошла ошибка при обнаружении или создании файла конфигурации: {ex.Message}");
                SetTextOutputForWin("! Автоматичские действия прерваны.");
            }

            // запускаем в тихом режиме поиск картинок в дирректории Windows

            return _xmlData;
        }

        /// <summary>
        /// Поиск картинок в системной папке для заставки Виндовс
        /// </summary>
        /// <returns></returns>
        public List<string> SearchFilesInWindowsFolder()
        {
            var photoFullFilesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _pathToPicturesLocal);

            //SetTextOutputForWin($"GetFolderPath: {photoFullFilesPath}");

            var allFilesToFolder = new DirectoryInfo(photoFullFilesPath).GetFiles().ToList();

            // новый массив для выбранных по размеру файлов из следующего перебора
            List<string> pathGoodPhotos = new List<string>();

            // перебираем файлы в папке
            foreach (var item in allFilesToFolder)
            {
                var isImages = _imageService.IsImage(item.FullName);

                if (isImages)
                {
                    using (Bitmap bitmap = new Bitmap(item.FullName))
                    {
                        // отбираем картинки только с шириной минимум 1900
                        if (bitmap.Width > 1900)
                        //if (item.Length > 1900) // для портретного режима
                        {
                            pathGoodPhotos.Add(item.FullName);
                        }
                    }

                }
            }
            return pathGoodPhotos;
        }

        private void XmlConafigurationFile()
        {

        }

        /// <summary>
        /// выводит текст на Label в окне, каждый вызов создает + новую строку
        /// </summary>
        /// <param name="setText"></param>
        /// <returns></returns>
        public bool SetTextOutputForWin(string setText)
        {
            _mainWindow.OutputForWin.Text = $"{setText}\n" +
                $"\n{_mainWindow.OutputForWin.Text}";
            // todo добавить проверку выведения текста, здесь за этим возвращаемое значение
            return true;
        }

        private bool IsPicture()
        {
            return true;
        }

        public void SaveMethod(List<string> pathGoodPhotos, string pathSaveImages)
        {
            int k = 0; // переменная для имени файла (временное значение)

            // Проверяем фотото на наличие копий, уже имеющихся в папке назначения.
            CheckingPhotosForCopies(ref pathGoodPhotos, pathSaveImages);

            if (pathGoodPhotos.Count == 0)
            {
                SetTextOutputForWin("Все найденные изображения уже есть в выбранной папке.");
                return;
            }

            foreach (var pathGoodPhoto in pathGoodPhotos)
            {
                var q = File.GetCreationTime(pathGoodPhoto).ToShortDateString();
                var newPath = Path.Combine(
                    pathSaveImages,
                    String.Concat(
                        "Photo_",
                        File.GetCreationTime(pathGoodPhoto).ToString().Replace(':', '-').Replace(' ', '_'),
                        "_", "(" + k + ")", ".jpg"));

                File.Copy(pathGoodPhoto, newPath, true);
                k++;

                SetTextOutputForWin($"Копирование в {newPath} выполнено успешно");
            }
        }

        /// <summary>
        /// Проверяет наличие копий картинок в данных директориях.
        /// </summary>
        /// <param name="pathGoodPhotos">Коллекция адресов из одной дирректории</param>
        /// <param name="folderBrowserDialog">Выбранная папка для копирования в нее картинок</param>
        private static void CheckingPhotosForCopies(ref List<string> pathGoodPhotosOfImageInTheSystemDirectory, string pathSaveImages)
        {
            // Получаем список файлов картинок в выбранной директории.
            var listOfImageFiles = Directory.GetFiles(pathSaveImages, "*.jpg").ToList();

            // Получаем перцептивный хеш картинок в выбранной директории.

            // Коллекция перцептивного хеша картинок в выбранной директории.
            var perceptualHashOfImagesInTheCollection = new List<int[]>();

            // todo здесь мы съедаем 2 гб памати в форыче за счет значимых переменных

            // Перебираем каждый адрес к картинке в выбранной директории.

            // Добавляем перцептивный хеш каждой картинки в коллекцию.
            perceptualHashOfImagesInTheCollection.AddRange(new PerceptualHash().PerceptualHashOfImages(listOfImageFiles));


            // Адреса картинок, которые уже есть в выбранной дирректории.
            var existingImageURLs = new List<string>();

            // Перебор коллекции адресов в системной директории
            foreach (var pathGoodPhotoOfImageInTheSystemDirectory in pathGoodPhotosOfImageInTheSystemDirectory)
            {
                // Получаем перцептивный хеш одной картинки из системной директории.
                int[] perceptualHashOfImageInTheSystemDirectory = new PerceptualHash().PerceptualHashOfImage(pathGoodPhotoOfImageInTheSystemDirectory);

                // Сравниваем хеш каждой картинки из системной дирректории с хешом картинки из выбранной по каждому числу,
                // если хеш первой находит аналогию, то адрес этой картинки добавляем в сиписок, который 
                // далее вычтем из списка адресов.
                foreach (var perceptualHashImageInTheCollection in perceptualHashOfImagesInTheCollection)
                {
                    // Счетчик совпадений данных в массиве перцептивного хеша.
                    int coincidenceCounter = 0;

                    // Перебираем каждый массив данных по числу.
                    for (int i = 0; i < perceptualHashOfImageInTheSystemDirectory.Length; i++)
                    {
                        // Если цифра с одинаковым индексом в хешах совпадает, добавляем 1 к счетчику.
                        if (perceptualHashOfImageInTheSystemDirectory[i] == perceptualHashImageInTheCollection[i])
                        {
                            coincidenceCounter++;
                        }
                    }

                    // Если совпадение составляет больше 62 из 64, то мы считаем, что фото идентичное
                    // и добавляем в коллекцию адресов уже существующих картинок.
                    if (coincidenceCounter > 63)
                    {
                        existingImageURLs.Add(pathGoodPhotoOfImageInTheSystemDirectory);
                    }
                }
            }

            // Проверяем, если не пустая коллекция адресов с идентичными картинками, то
            if (existingImageURLs.Count != 0)
            {
                // Перебираем коллекцию адресов на удаление.
                foreach (var item in existingImageURLs)
                {
                    // Удаляем по одному адресу из коллекции.
                    pathGoodPhotosOfImageInTheSystemDirectory.Remove(item);
                }
            }
        }


    }
}
