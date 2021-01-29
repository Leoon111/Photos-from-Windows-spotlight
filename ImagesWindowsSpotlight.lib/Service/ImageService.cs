using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImagesWindowsSpotlight.lib.Service
{
    public class ImageService : IImagesService
    {
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

        public List<byte[]> getPerceptualHashOfImagesList(List<string> pathImagesList)
        {
            throw new NotImplementedException();
        }

        public async Task SaveImagesToAsync(List<string> imagePaths, string saveFolder)
        {
            throw new NotImplementedException();
        }
    }
}
