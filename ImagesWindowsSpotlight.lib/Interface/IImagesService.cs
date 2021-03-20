﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImagesWindowsSpotlight.lib.Models;

namespace ImagesWindowsSpotlight.lib
{
    public interface IImagesService
    {
        /// <summary>
        /// Определение, является ли файл изображением
        /// </summary>
        /// <param name="filePath">полный путь к файлу</param>
        /// <returns>изображение ли</returns>
        bool IsImage(string filePath);

        /// <summary>
        /// Проверка наличия дубликата изображения в папке
        /// </summary>
        /// <param name="filePath">путь к файлу изображения</param>
        /// <param name="pathFolder">путь к папке, где изображения с которыми сравниваем</param>
        /// <returns>есть ли дубликат изображения в паке</returns>
        bool ImageExistToDir(string filePath, string pathFolder);

        /// <summary>
        /// Сравнение двух изображений на идентичность по алгоритму
        /// </summary>
        /// <param name="filePath_1">путь к изображению 1</param>
        /// <param name="filePath_2">путь к изображению 2</param>
        /// <returns>являются ли изображения одинаковыми</returns>
        bool ImagesCompare(string filePath_1, string filePath_2);

        /// <summary>
        /// Добавление перцептивного хеша в объект класса PHashAndDataImage
        /// </summary>
        /// <param name="imageData">Данные о изображении без перцептивного хеша</param>
        void GetPerceptualHashOfImageData(PHashAndDataImage imageData);

        /// <summary>
        /// Получение перцептивного хеша по пути изображения
        /// </summary>
        /// <param name="imagesPath">путь к изображению</param>
        /// <returns>перцептивный хеш изображения</returns>
        byte[] GetPerceptualHashOfImage(string imagesPath);

        /// <summary>
        /// Получение перцептивного хеша по коллекции путей изображений
        /// </summary>
        /// <param name="pathImagesList">коллекция адресов изображений на диске</param>
        /// <returns>коллекция перцептивных хешей</returns>
        List<PHashAndDataImage> GetPerceptualHashOfImagesList(List<FileInfo> pathImagesList, CancellationToken cancellation);

        /// <summary>
        /// Поиск изображений в выбранной папке
        /// </summary>
        /// <param name="pathFolder">путь к папке</param>
        /// <returns>коллекция изображений</returns>
        List<PHashAndDataImage> SearchImagesInFolder(string pathFolder);
        
        /// <summary>
        /// Асинхронное сохранение изображений на диск
        /// </summary>
        /// <param name="imagePaths">коллекция адресов изображений</param>
        /// <param name="saveFolder">папка для сохранения</param>
        /// <returns></returns>
        Task SaveImagesToAsync(List<string> imagePaths, string saveFolder);
    }
}
