using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photos_Windows_spotlight
{
    class Configuration
    {
        private string _pathFile;
        private List<PhotoData> photoDatas;

        public string PathFile { get => _pathFile; set => _pathFile = value; }

        public List<PhotoData> PhotoDatas { get => photoDatas; set => photoDatas = value; }
    }
}
