using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace Photos_Windows_spotlight
{
    class StartProgram
    {
        private MainWindow _mainWindow;
        private XMLData _xmlData;
        private Configuration _configuration;

        public StartProgram()
        {
            _xmlData = new XMLData();
            // ? проверяем, если в настройках есть "Запускать проверку в фоне при старет", то старуем
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

        public List<string> SearchFilesInWindowsFolder()
        {
            var photoFilesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets"
                                             );

            SetTextOutputForWin($"GetFolderPath: {photoFilesPath}");

            /// получение списка файлов в директории.
            var allPhotoFiles = new DirectoryInfo(photoFilesPath).GetFiles().ToList();


            // новый массив для выбранных по размеру файлов из следующего перебора
            List<string> pathGoodPhotos = new List<string>();

            /// перебираем файлы в папке
            foreach (var item in allPhotoFiles)
            {
                /// ограничиваем размер файла для того, чтоб не попали ненужные фалы, не картинки
                /// это предыдущее решение(глупое, но простое), переделать на определение картинки
                if (item.Length > 200000)
                {
                    using (Bitmap bitmap = new Bitmap(item.FullName))
                    {

                        /// отбираем картинки только с шириной минимум 1900
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
            _mainWindow.OutputForWin.Text += $"\n{setText}";
            // todo добавить проверку выведения текста, здесь за этим возвращаемое значение
            return true;
        }

        private bool IsPicture()
        {
            return true;
        }

        public void SaveMethod(List<string> pathGoodPhotos, FolderBrowserDialog folderBrowserDialog)
        {
            int k = 0; // переменная для имени файла (временное значение)

            /// Проверяем фотото на наличие копий, уже имеющихся в папке назначения.
            CheckingPhotosForCopies(ref pathGoodPhotos, folderBrowserDialog);

            if (pathGoodPhotos.Count == 0)
            {
                SetTextOutputForWin("Нет не одной новой картинки.\n");
                return;
            }

            foreach (var pathGoodPhoto in pathGoodPhotos)
            {
                var q = File.GetCreationTime(pathGoodPhoto).ToShortDateString();
                var newPath = Path.Combine(
                    folderBrowserDialog.SelectedPath,
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
        private static void CheckingPhotosForCopies(ref List<string> pathGoodPhotosOfImageInTheSystemDirectory, FolderBrowserDialog folderBrowserDialog)
        {
            /// Получаем список файлов картинок в выбранной директории.
            var listOfImageFiles = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.jpg").ToList();

            /// Получаем перцептивный хеш картинок в выбранной директории.

            /// Коллекция перцептивного хеша картинок в выбранной директории.
            var perceptualHashOfImagesInTheCollection = new List<int[]>();

            /// Перебираем каждый адрес к картинке в выбранной директории.
            foreach (var pathToPicture in listOfImageFiles)
            {
                /// Добавляем перцептивный хеш каждой картинки в коллекцию.
                perceptualHashOfImagesInTheCollection.Add(PerceptualHashOfImage(pathToPicture));
            }

            /// Адреса картинок, которые уже есть в выбранной дирректории.
            var existingImageURLs = new List<string>();

            /// Перебор коллекции адресов в системной директории
            foreach (var pathGoodPhotoOfImageInTheSystemDirectory in pathGoodPhotosOfImageInTheSystemDirectory)
            {
                /// Получаем перцептивный хеш одной картинки из системной директории.
                int[] perceptualHashOfImageInTheSystemDirectory = PerceptualHashOfImage(pathGoodPhotoOfImageInTheSystemDirectory);

                /// Сравниваем хеш каждой картинки из системной дирректории с хешом картинки из выбранной по каждому числу,
                /// если хеш первой находит аналогию, то адрес этой картинки добавляем в сиписок, который 
                /// далее вычтем из списка адресов.
                foreach (var perceptualHashImageInTheCollection in perceptualHashOfImagesInTheCollection)
                {
                    /// Счетчик совпадений данных в массиве перцептивного хеша.
                    int coincidenceCounter = 0;

                    /// Перебираем каждый массив данных по числу.
                    for (int i = 0; i < perceptualHashOfImageInTheSystemDirectory.Length; i++)
                    {
                        /// Если цифра с одинаковым индексом в хешах совпадает, добавляем 1 к счетчику.
                        if (perceptualHashOfImageInTheSystemDirectory[i] == perceptualHashImageInTheCollection[i])
                        {
                            coincidenceCounter++;
                        }
                    }

                    /// Если совпадение составляет больше 62 из 64, то мы считаем, что фото идентичное
                    /// и добавляем в коллекцию адресов уже существующих картинок.
                    if (coincidenceCounter > 63)
                    {
                        existingImageURLs.Add(pathGoodPhotoOfImageInTheSystemDirectory);
                    }
                }
            }

            /// Проверяем, если не пустая коллекция адресов с идентичными картинками, то
            if (existingImageURLs.Count != 0)
            {
                /// Перебираем коллекцию адресов на удаление.
                foreach (var item in existingImageURLs)
                {
                    /// Удаляем по одному адресу из коллекции.
                    pathGoodPhotosOfImageInTheSystemDirectory.Remove(item);
                }
            }
        }

        /// <summary>
        /// Метод получения перцептивного хеша из картинки
        /// </summary>
        /// <param name="pathToPicture">Адрес расположения картинки</param>
        /// <returns>Массив перцептивного хеша картинки</returns>
        private static int[] PerceptualHashOfImage(string pathToPicture)
        {
            /// Уменьшаем картинку до размеров 8х8.
            var miniImage = new Bitmap(Image.FromFile(pathToPicture), 8, 8);
            //miniImage.Save(item + ".jpg");

            /// массив значений пикселей, равный колличеству пикселей на перцептивном хеше
            int[] sumOfPixelValues = new int[64];
            int pixelNumber = 0;

            /// Преобразование уменшенного изображения в градиент серого воспользовавшись формулой перевода RGB в YUV
            /// Из нее нам потребуется компонента Y, формула конвертации которой выглядит так: Y = 0.299 x R + 0.587 x G + B x 0.114
            for (int x = 0; x < miniImage.Width; x++)
            {
                for (int y = 0; y < miniImage.Height; y++)
                {
                    Color bitmapColor = miniImage.GetPixel(x, y);
                    int colorGray = (int)(bitmapColor.R * 0.299 +
                    bitmapColor.G * 0.587 + bitmapColor.B * 0.114);
                    miniImage.SetPixel(x, y, Color.FromArgb(colorGray, colorGray, colorGray));
                    sumOfPixelValues[pixelNumber++] = colorGray;
                }
            }

            /// Вычислите среднее значение для всех 64 пикселей уменьшенного изображения
            var averageSumOfPixelValues = sumOfPixelValues.AsQueryable().Average();

            /// Заменяем каждое знанеие цвета пикселя на 1 или 0 в зависимости от того, больше оно среднего значения или меньше
            for (int i = 0; i < sumOfPixelValues.Length; i++)
            {
                if (sumOfPixelValues[i] >= averageSumOfPixelValues)
                {
                    sumOfPixelValues[i] = 1;
                }
                else
                {
                    sumOfPixelValues[i] = 0;
                }
            }

            return sumOfPixelValues;
        }
    }
}
