using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        /// Получение перцептивного хеша изображения
        /// </summary>
        /// <param name="imagesPath">путь к изображению</param>
        /// <returns>перцептивный хеш изображения</returns>
        byte[] GetPerceptualHashOfImage(string imagesPath);

        /// <summary>
        /// Получение перцептивного хеша коллекции изображений
        /// </summary>
        /// <param name="pathImagesList">коллекция адресов изображений на диске</param>
        /// <returns>коллекция перцептивных хешей</returns>
        List<PHashAndNames> GetPerceptualHashOfImagesList(List<FileInfo> pathImagesList);

        /// <summary>
        /// Поиск изображений в выбранной папке
        /// </summary>
        /// <param name="pathFolder">путь к папке</param>
        /// <returns>коллекция изображений</returns>
        List<ImageInfo> SearchImagesInFolder(string pathFolder);

        /// <summary>
        /// Асинхронное сохранение изображений на диск
        /// </summary>
        /// <param name="imagePaths">коллекция адресов изображений</param>
        /// <param name="saveFolder">папка для сохранения</param>
        /// <returns></returns>
        Task SaveImagesToAsync(List<string> imagePaths, string saveFolder);
    }
}
