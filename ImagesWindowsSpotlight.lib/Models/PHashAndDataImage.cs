using ImagesWindowsSpotlight.lib.Models.Base;

namespace ImagesWindowsSpotlight.lib.Models
{
    ///<summary>Перцептивный хеш и имя файла</summary>
    public class PHashAndDataImage : ImageInfo
    {
        /// <summary>Перцептивный хеш изображения</summary>
        public byte[] PerceptualHash { get; set; }

        /// <summary>Путь к файлу изображения на диске</summary>
        public string PathImageFile { get; set; }

        /// <summary>Файл изображения</summary>
        public byte[] ImageData { get; set; }
    }
}
