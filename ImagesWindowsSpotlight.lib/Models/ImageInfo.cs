using System;
using System.Drawing;

namespace ImagesWindowsSpotlight.lib.Models
{
    public class ImageInfo
    {
        /// <summary>Имя изображения</summary>
        public string Name {get; set; }

        /// <summary>Разрешение изображения</summary>
        public Size Resolution { get; set; }

        /// <summary>Файл изображения</summary>
        public Bitmap ImageData { get; set; }

        /// <summary>Дата создания</summary>
        public DateTime DateOfCreation { get; set; }

        // todo добавить тип изображения
    }
}
