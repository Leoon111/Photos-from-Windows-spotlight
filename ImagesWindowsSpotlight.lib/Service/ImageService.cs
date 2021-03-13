using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagesWindowsSpotlight.lib.Models;

namespace ImagesWindowsSpotlight.lib.Service
{
    public class ImageService : IImagesService
    {
        // массив значений пикселей, равный количеству пикселей на перцептивном хеше
        //private int[] _sumOfPixelValues;
        //private Bitmap _miniImage;
        //private int _pixelNumber;
        //private Color _bitmapColor;

        public ImageService()
        {
            //_sumOfPixelValues = new int[64];
        }

        /// <summary>
        /// Поиск изображений в выбранной папке
        /// </summary>
        /// <param name="pathFolder">путь к папке</param>
        /// <returns>коллекция изображений</returns>
        public List<PHashAndDataImage> SearchImagesInFolder(string pathFolder)
        {
            var newImagesList = new List<PHashAndDataImage>();

            var photoFullFilesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), pathFolder);

            var allFilesToFolder = new DirectoryInfo(photoFullFilesPath).GetFiles().ToList();

            foreach (var item in allFilesToFolder)
            {
                if (IsImage(item.FullName))
                {
                    var imageBitmap = new Bitmap(item.FullName);

                    var image = new PHashAndDataImage
                    {
                        Name = Path.ChangeExtension(item.Name, ".jpg"),
                        DateOfCreation = item.CreationTime,
                        Resolution = imageBitmap.Size,
                        ImageBitmap = imageBitmap,
                    };
                    newImagesList.Add(image);
                }
            }
            return newImagesList;
        }

        /// <summary>
        /// Определение, является ли файл изображением
        /// </summary>
        /// <param name="filePath">полный путь к файлу</param>
        /// <returns>изображение ли</returns>
        public bool IsImage(string filePath)
        {
            // на данный момент проверка только на jpeg, т.к. другие не требуются
            return ImageCheck.HasJpegHeader(filePath);
        }

        public bool ImageExistToDir(string filePath, string pathFolder)
        {
            throw new NotImplementedException();
        }

        public bool ImagesCompare(string filePath_1, string filePath_2)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Получение перцептивного хеша из данных Bitmap изображения
        ///// </summary>
        ///// <param name="imagesPath">путь к изображению</param>
        ///// <returns>перцептивный хеш изображения</returns>
        //public byte[] GetPerceptualHashOfImageData(Bitmap imageBitmap)
        //{
        //    // Уменьшаем картинку до размеров 8х8.
        //    var miniImage = new Bitmap(imageBitmap, 8, 8);

        //    return GetPerceptualHash(miniImage);
        //}

        /// <summary>
        /// Добавление перцептивного хеша в объект класса PHashAndDataImage
        /// </summary>
        /// <param name="imageData">Данные о изображении без перцептивного хеша</param>
        public void GetPerceptualHashOfImageData(PHashAndDataImage imageData)
        {
            if (imageData.PerceptualHash is null && imageData.PathImageFile is null && imageData.ImageBitmap != null && imageData.Resolution.Width > 900)
            {
                var pHash = GetPerceptualHash(new Bitmap(imageData.ImageBitmap, 8, 8));
                imageData.PerceptualHash = pHash;
            }
        }

        /// <summary>
        /// Получение перцептивного хеша по пути изображения
        /// </summary>
        /// <param name="imagesPath">путь к изображению</param>
        /// <returns>перцептивный хеш изображения</returns>
        public byte[] GetPerceptualHashOfImage(string imagesPath)
        {
            // Уменьшаем картинку до размеров 8х8.
            var miniImage = new Bitmap(Image.FromFile(imagesPath), 8, 8);

            return GetPerceptualHash(miniImage);
        }

        /// <summary>
        /// Получение перцептивного хеша по коллекции путей изображений
        /// </summary>
        /// <param name="pathImagesList">коллекция адресов изображений на диске</param>
        /// <returns>коллекция перцептивных хешей</returns>
        public List<PHashAndDataImage> GetPerceptualHashOfImagesList(List<FileInfo> pathImagesList, CancellationToken cancellation = default)
        {
            var pHashImages = new ConcurrentBag<PHashAndDataImage>();
            ThreadPool.SetMinThreads(8, 4);
            Parallel.ForEach(pathImagesList, new ParallelOptions { MaxDegreeOfParallelism = 3 }, pathImage =>
              {
                  if (IsImage(pathImage.FullName))
                  {
                      cancellation.ThrowIfCancellationRequested();
                      pHashImages.Add(
                          new PHashAndDataImage
                          {
                              PerceptualHash = GetPerceptualHashOfImage(pathImage.FullName),
                              Name = pathImage.Name,
                              DateOfCreation = pathImage.CreationTime,
                              DateLastChange = pathImage.LastWriteTime,
                              });
                  }
              });
            // todo ввести везде потокобезопасную коллекцию
            return pHashImages.ToList();
        }

        /// <summary>
        /// Получение хеша из уменьшенной коллекции байт изображения 8х8
        /// </summary>
        /// <param name="miniImage"></param>
        /// <returns>перцептивный хеш изображения</returns>
        private byte[] GetPerceptualHash(Bitmap miniImage)
        {
            int _pixelNumber = 0;
            var sumOfPixelValues = new int[64];
            // Преобразование уменьшенного изображения в градиент серого воспользовавшись формулой перевода RGB в YUV
            // Из нее нам потребуется компонента Y, формула конвертации которой выглядит так: Y = 0.299 x R + 0.587 x G + B x 0.114
            for (int x = 0; x < miniImage.Width; x++)
            {
                for (int y = 0; y < miniImage.Height; y++)
                {
                    var bitmapColor = miniImage.GetPixel(x, y);
                    int colorGray = (int)(bitmapColor.R * 0.299 +
                                          bitmapColor.G * 0.587 + bitmapColor.B * 0.114);
                    miniImage.SetPixel(x, y, Color.FromArgb(colorGray, colorGray, colorGray));
                    sumOfPixelValues[_pixelNumber++] = colorGray;
                }
            }
            // Вычислите среднее значение для всех 64 пикселей уменьшенного изображения
            var averageSumOfPixelValues = sumOfPixelValues.AsQueryable().Average();
            var pHash = new byte[64];
            // Заменяем каждое значение цвета пикселя на 1 или 0 в зависимости от того, больше оно среднего значения или меньше
            for (int i = 0; i < sumOfPixelValues.Length; i++)
            {
                if (sumOfPixelValues[i] >= averageSumOfPixelValues) pHash[i] = 1;
                else pHash[i] = 0;
            }
            return pHash;
        }

        /// <summary>
        /// Асинхронное сохранение изображений на диск
        /// </summary>
        /// <param name="imagePaths">коллекция адресов изображений</param>
        /// <param name="saveFolder">папка для сохранения</param>
        /// <returns></returns>
        public async Task SaveImagesToAsync(List<string> imagePaths, string saveFolder)
        {
            throw new NotImplementedException();
        }
    }
}
