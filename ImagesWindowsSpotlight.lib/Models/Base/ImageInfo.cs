using System;
using System.Drawing;

namespace ImagesWindowsSpotlight.lib.Models.Base
{
    ///<summary>Вся нужная информация о изображении и его данные</summary>
    public class ImageInfo
    {
        /// <summary>Имя изображения</summary>
        public string Name { get; set; }

        /// <summary>Разрешение изображения</summary>
        public ResolutionImage Resolution { get; set; }

        /// <summary>Дата создания</summary>
        public DateTime DateOfCreation { get; set; }

        /// <summary>Дата последнего изменения</summary>
        public DateTime DateLastChange { get; set; }
        // todo добавить тип изображения
    }
}
