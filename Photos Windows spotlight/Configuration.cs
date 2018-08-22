using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Photos_Windows_spotlight
{
    class Configuration
    {
        public string PathFiles { get; set; }

        [XmlArray("PhotoDatas")]
        public List<PhotoData> PhotoDatas { get; set; }
    }
}
