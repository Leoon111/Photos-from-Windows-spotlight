using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photos_from_Windows_spotlight
{
    /// <summary>
    /// Класс с данными о фотографиях для сохранения в БД.
    /// </summary>
    class PhotoData
    {
        #region Private fields
        private string name;
        private int size;
        private DateTime fileDate;
        private string previousName;
        #endregion

        #region Public constructors
        public string Name
        {
            get => name;
            set => name = value;
        }
        public int Size
        {
            get => size;
            set => size = value;
        }
        public DateTime FileDate
        {
            get => fileDate;
            set => fileDate = value;
        }
        public string PreviousName
        {
            get => previousName;
            set => previousName = value;
        }
        #endregion
    }

}
