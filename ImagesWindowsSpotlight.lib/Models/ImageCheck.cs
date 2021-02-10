using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImagesWindowsSpotlight.lib.Service
{
    public class ImageCheck
    {
        /// <summary>
        /// проверка, является ли файл форматом jpeg
        /// </summary>
        /// <param name="filename">полный путь к файлу</param>
        /// <returns></returns>
        public static bool HasJpegHeader(string filename)
        {
            using (var br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
            {
                UInt16 soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                UInt16 marker = br.ReadUInt16(); // JFIF marker (FFE0) or EXIF marker(FFE1)

                return soi == 0xd8ff && (marker & 0xe0ff) == 0xe0ff;
            }
        }
    }
}
