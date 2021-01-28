using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photos_Windows_spotlight
{
    public class MyImages
    {

        public static bool IsImage(string fileName)
        {
            var isImg = HasJpegHeader(fileName);


            return isImg;
        }

        static bool HasJpegHeader(string filename)
        {
            using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
            {
                UInt16 soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                UInt16 marker = br.ReadUInt16(); // JFIF marker (FFE0) or EXIF marker(FFE1)

                return soi == 0xd8ff && (marker & 0xe0ff) == 0xe0ff;
            }
        }
    }
}
