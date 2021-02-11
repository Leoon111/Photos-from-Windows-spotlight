using System;
using System.Drawing;

namespace ImagesWindowsSpotlight.lib.Models
{
    ///<summary>Вся нужная информация о изображении и его данные</summary>
    public class ImageInfo
    {
        /// <summary>Имя изображения</summary>
        public string Name {get; set; }

        /// <summary>Разрешение изображения</summary>
        public ResolutionImage Resolution { get; set; }

        /// <summary>Файл изображения</summary>
        public byte[] ImageData { get; set; }

        /// <summary>Дата создания</summary>
        public DateTime DateOfCreation { get; set; }

        // todo добавить тип изображения
    }
}
