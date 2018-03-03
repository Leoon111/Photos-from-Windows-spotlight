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

            Console.WriteLine("GetFolderPath: {0}", photoFilesPath);

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

            /// Тестовая папка на моем компе, проверяю, если ее нет, то открываем Мой Компьютер
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
            /// если не выбрали папку для расположения файлов
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
    }
}
