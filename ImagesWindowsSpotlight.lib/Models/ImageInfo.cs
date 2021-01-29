using System;
using System.Drawing;

namespace ImagesWindowsSpotlight.lib.Models
{
    public class ImageInfo
    {
        public string Name {get; set; }

        public Size Resolution { get; set; }

        public byte[] ImageData { get; set; }

        public DateTime DateOfCreation { get; set; }

        // todo добавить тип изображения
    }
}
