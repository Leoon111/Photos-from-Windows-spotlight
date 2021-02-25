﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImagesWindowsSpotlight.lib.Models;

namespace ImagesWindowsSpotlight.lib.Service
{
    public class ImageService : IImagesService
    {
        // массив значений пикселей, равный колличеству пикселей на перцептивном хеше
        private int[] _sumOfPixelValues;
        private Bitmap _miniImage;
        //private int _pixelNumber;
        private Color _bitmapColor;

        public ImageService()
        {
            _sumOfPixelValues = new int[64];
        }

        /// <summary>
        /// Поиск изображений в выбранной папке
        /// </summary>
        /// <param name="pathFolder">путь к папке</param>
        /// <returns>коллекция изображений</returns>
        public List<ImageInfo> SearchImagesInFolder(string pathFolder)
        {
            var newImagesList = new List<ImageInfo>();

            var photoFullFilesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), pathFolder);

            var allFilesToFolder = new DirectoryInfo(photoFullFilesPath).GetFiles().ToList();

            foreach (var item in allFilesToFolder)
            {
                if (IsImage(item.FullName))
                {
                    // таким способом я отвязываю изображение от ссылки, на данный момент не знаю другого
                    var imageDate = File.ReadAllBytes(item.FullName);
                    Size resolution;
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(imageDate, 0, imageDate.Length);
                        resolution = Image.FromStream(ms).Size;
                    }

                    var image = new ImageInfo
                    {
                        Name = Path.ChangeExtension(item.Name, ".jpg"),
                        DateOfCreation = item.CreationTime,
                        Resolution = new ResolutionImage
                        {
                            Width = resolution.Width,
                            Height = resolution.Height
                        },
                        ImageData = imageDate,
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

        /// <summary>
        /// Получение перцептивного хеша изображения
        /// </summary>
        /// <param name="imagesPath">путь к изображению</param>
        /// <returns>перцептивный хеш изображения</returns>
        public byte[] GetPerceptualHashOfImage(string imagesPath)
        {
            // Уменьшаем картинку до размеров 8х8.
            _miniImage = new Bitmap(Image.FromFile(imagesPath), 8, 8);
            
            int _pixelNumber = 0;

            // Преобразование уменьшенного изображения в градиент серого воспользовавшись формулой перевода RGB в YUV
            // Из нее нам потребуется компонента Y, формула конвертации которой выглядит так: Y = 0.299 x R + 0.587 x G + B x 0.114
            for (int x = 0; x < _miniImage.Width; x++)
            {
                for (int y = 0; y < _miniImage.Height; y++)
                {
                    _bitmapColor = _miniImage.GetPixel(x, y);
                    int colorGray = (int)(_bitmapColor.R * 0.299 +
                                          _bitmapColor.G * 0.587 + _bitmapColor.B * 0.114);
                    _miniImage.SetPixel(x, y, Color.FromArgb(colorGray, colorGray, colorGray));
                    _sumOfPixelValues[_pixelNumber++] = colorGray;
                }
            }

            // Вычислите среднее значение для всех 64 пикселей уменьшенного изображения
            var averageSumOfPixelValues = _sumOfPixelValues.AsQueryable().Average();
            var pHash = new byte[64];
            // Заменяем каждое значение цвета пикселя на 1 или 0 в зависимости от того, больше оно среднего значения или меньше
            for (int i = 0; i < _sumOfPixelValues.Length; i++)
            {
                if (_sumOfPixelValues[i] >= averageSumOfPixelValues) pHash[i] = 1;
                else pHash[i] = 0;
            }
            return pHash;
        }

        /// <summary>
        /// Получение перцептивного хеша коллекции изображений
        /// </summary>
        /// <param name="pathImagesList">коллекция адресов изображений на диске</param>
        /// <returns>коллекция перцептивных хешей</returns>
        public List<PHashAndNames> GetPerceptualHashOfImagesList(List<FileInfo> pathImagesList)
        {
            var pHashAndNames = new List<PHashAndNames>();
            foreach (var images in pathImagesList)
            {
                if (IsImage(images.FullName))
                {
                    pHashAndNames.Add(
                        new PHashAndNames
                        {
                            PerceptualHash = GetPerceptualHashOfImage(images.FullName),
                            Name = images.Name,
                        });
                }
            }
            return pHashAndNames;
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
