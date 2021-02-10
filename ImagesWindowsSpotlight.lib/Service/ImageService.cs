using System;
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

        public byte[] GetPerceptualHashOfImage(string imagesPath)
        {
            throw new NotImplementedException();
        }

        public List<byte[]> GetPerceptualHashOfImagesList(List<string> pathImagesList)
        {
            throw new NotImplementedException();
        }

        public async Task SaveImagesToAsync(List<string> imagePaths, string saveFolder)
        {
            throw new NotImplementedException();
        }
    }
}
