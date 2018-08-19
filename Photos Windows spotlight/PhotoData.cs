using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photos_Windows_spotlight
{
    /// <summary>
    /// Класс с данными о фотографиях для сохранения в БД.
    /// </summary>
    class PhotoData
    {
        #region Private fields

        /// <summary>
        /// имя файла, которым мы его сохранили
        /// </summary>
        private string _previouslySavedName;

        /// <summary>
        /// дата перовй обработки файла
        /// </summary>
        private DateTime _fileDate;

        /// <summary>
        /// перцептивный хэш картинки
        /// </summary>
        private string _perceptualHashOfImages;

        #endregion

        #region Public constructors

        public string PreviouslySavedName { get => _previouslySavedName; set => _previouslySavedName = value; }
        public DateTime FileDate { get => _fileDate; set => _fileDate = value; }
        public string PerceptualHashOfImages { get => _perceptualHashOfImages; set => _perceptualHashOfImages = value; }


        #endregion
    }

}
