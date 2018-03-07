using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Photos_from_Windows_spotlight
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {

            /// формирование адреса расположения файлов фотографий в системе.
            var photoFilesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets"
                                             );

            //Console.WriteLine("GetFolderPath: {0}", photoFilesPath);

            /// получение списка файлов в директории.
            var allPhotoFiles = new DirectoryInfo(photoFilesPath).GetFiles().ToList();

            ///Delay
            //Console.ReadKey();

            // новый массив для выбранных по размеру файлов из следующего перебора
            List<string> pathGoodPhotos = new List<string>();

            /// перебираем файлы в папке
            foreach (var item in allPhotoFiles)
            {
                /// ограничиваем размер файла для того, чтоб не попали ненужные фалы, не картинки
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

            // выбрать место сохнанения.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите папку, куда будут сохраняться картинки";
            /// Тестовая папка на моем компе, проверяю, если ее нет, то открываем Мой Компьютер (если не мой компьютер)
            folderBrowserDialog.SelectedPath =
                Directory.Exists(@"E:\OneDrive\Новые фотографии\1\test\1\")
                ? @"E:\OneDrive\Новые фотографии\1\test\1\"
                : Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);


            /// метод сохранения
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("Картинки сохраняем по адресу:\n {0}\n", folderBrowserDialog.SelectedPath);

                /// скопировать и переименовать файлы
                SaveMethod(pathGoodPhotos, folderBrowserDialog);
            }

            /// Сообщение, если не выбрали папку для расположения файлов
            else
            {
                Console.WriteLine("\nКопирование не выполнено по причине невыбранной папки назначения");
            }
            folderBrowserDialog.Dispose();



            Console.WriteLine("\nКопирование всех картинок выполенно успешно, нажмите любую кнопку");
            Console.ReadKey();
        }

        /// <summary>
        /// Переименовывает и сохраняте файлы в указанную пользователем папку.
        /// </summary>
        /// <param name="pathGoodPhotos">Коллекция путей к файлам картинок в системной директории компьютера</param>
        /// <param name="folderBrowserDialog">Выбранная папка пользователем для сохранния картинок</param>
        private static void SaveMethod(List<string> pathGoodPhotos, FolderBrowserDialog folderBrowserDialog)
        {
            int k = 0; // переменная для имени файла (временное значение)

            /// Проверяем фотото на наличие копий, уже имеющихся в папке назначения.
            CheckingPhotosForCopies(ref pathGoodPhotos, folderBrowserDialog);

            foreach (var pathGoodPhoto in pathGoodPhotos)
            {
                var q = File.GetCreationTime(pathGoodPhoto).ToShortDateString();
                var newPath = Path.Combine(
                    folderBrowserDialog.SelectedPath,
                    String.Concat(
                        "Photo_",
                        File.GetCreationTime(pathGoodPhoto).ToString().Replace(':', '-').Replace(' ', '_'),
                        "_",
                        "(" + k + ")",
                        ".jpg"
                                  )
                                           );

                File.Copy(pathGoodPhoto, newPath, true);
                k++;

                Console.WriteLine("Копирование в {0} выполнено успешно", newPath);
            }
        }

        private static void CheckingPhotosForCopies(ref List<string> pathGoodPhotos, FolderBrowserDialog folderBrowserDialog)
        {
            /// Получаем список файлов картинок в выбранной директории.
            var listOfImageFiles = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.jpg").ToList();

            /// Получаем перцептивный хеш картинок в выбранной директории.
            var perceptualHashOfImages = new List<Bitmap>();
            foreach (var item in listOfImageFiles)
            {
                /// Уменьшаем картинку до размеров 8х8.
                var miniImage = new Bitmap(Image.FromFile(item), 8, 8);
                //miniImage.Save(item + ".jpg");

                int[] sumOfPixelValues = new int[64]; 

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
                        sumOfPixelValues[x + y] = colorGray;
                    }
                }
                //perceptualHashOfImages.Add(miniImage);

                /// Вычислите среднее значение для всех 64 цветов.
                var averageSumOfPixelValues = sumOfPixelValues.AsQueryable().Average();
                for (int i = 0; i < sumOfPixelValues.Length; i++)
                {
                    if (i >= averageSumOfPixelValues)
                    {
                        sumOfPixelValues[i] = 1;
                    }
                    else
                    {
                        sumOfPixelValues[i] = 0;
                    }
                }
                

                /// Получаем уменьшенных хеш картинок из системной директории.

                /// Сравниваем каждый хеш картинки из системной дирректории с хешом картинки из выбранной,
                /// если хеш первой находит аналогию, то его убираем из списка.



            }
        }
    }
}
