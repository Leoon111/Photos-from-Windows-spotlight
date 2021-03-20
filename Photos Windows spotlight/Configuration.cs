using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Photos_Windows_spotlight
{
    public class Configuration
    {
        public string PathFiles { get; set; }

        [XmlArray("PhotoDataset")]
        public List<PhotoData> PhotoDataset { get; set; }
    }
}
