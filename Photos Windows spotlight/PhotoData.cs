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
    public class PhotoData
    {

        #region Public constructors

        /// <summary>
        /// имя картинки при последнем сохнании
        /// </summary>
        public string PreviouslySavedName { get; set; }

        /// <summary>
        /// дата обнарушения фото
        /// </summary>
        public DateTime FileDate { get; set; }

        /// <summary>
        /// перцептивный хэш картинки
        /// </summary>
        public string PerceptualHashOfImages { get; set; }

        #endregion
    }

}
