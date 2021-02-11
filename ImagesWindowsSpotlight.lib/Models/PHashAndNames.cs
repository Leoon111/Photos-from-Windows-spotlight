using System;
using System.Collections.Generic;
using System.Text;

namespace ImagesWindowsSpotlight.lib.Models
{
    ///<summary>Перцептивный хеш и имя файла</summary>
    public class PHashAndNames
    {
        public string Name { get; set; }

        public byte[] PerceptualHash { get; set; }
    }
}
