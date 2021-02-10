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
                    var name = Path.ChangeExtension(item.Name, ".jpg");
                    var image = new ImageInfo
                    {
                        Name = Path.ChangeExtension(item.Name, ".jpg"),
                        DateOfCreation = item.CreationTime,
                        Resolution = Image.FromFile(item.FullName).Size,
                        ImageData = new Bitmap(item.FullName),
                    };
                    newImagesList.Add(image);
                }
            }
            return newImagesList;
        }

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
