using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photos_Windows_spotlight
{
    class PerceptualHash
    {
        private Bitmap miniImage;
        private int[] sumOfPixelValues;
        Color bitmapColor;

        public PerceptualHash()
        {

        }

        public List<int[]> PerceptualHashOfImages(List<string> pathToPictures)
        {
            var collectionPerceptualHash = new List<int[]>();
            foreach (var item in pathToPictures)
            {
                collectionPerceptualHash.Add(PerceptualHashOfImage(item));
            }
            return collectionPerceptualHash;
        }

        /// <summary>
        /// Метод получения перцептивного хеша из картинки
        /// </summary>
        /// <param name="pathToPicture">Адрес расположения картинки</param>
        /// <returns>Массив перцептивного хеша картинки</returns>
        public int[] PerceptualHashOfImage(string pathToPicture)
        {
            
            // Уменьшаем картинку до размеров 8х8.
            miniImage = new Bitmap(Image.FromFile(pathToPicture), 8, 8);
            //miniImage.Save(item + ".jpg");

            // todo вынести переменную
            // массив значений пикселей, равный колличеству пикселей на перцептивном хеше
            sumOfPixelValues = new int[64];
            int pixelNumber = 0;

            // Преобразование уменшенного изображения в градиент серого воспользовавшись формулой перевода RGB в YUV
            // Из нее нам потребуется компонента Y, формула конвертации которой выглядит так: Y = 0.299 x R + 0.587 x G + B x 0.114
            for (int x = 0; x < miniImage.Width; x++)
            {
                for (int y = 0; y < miniImage.Height; y++)
                {
                    bitmapColor = miniImage.GetPixel(x, y);
                    int colorGray = (int)(bitmapColor.R * 0.299 +
                    bitmapColor.G * 0.587 + bitmapColor.B * 0.114);
                    miniImage.SetPixel(x, y, Color.FromArgb(colorGray, colorGray, colorGray));
                    sumOfPixelValues[pixelNumber++] = colorGray;
                }
            }

            // Вычислите среднее значение для всех 64 пикселей уменьшенного изображения
            var averageSumOfPixelValues = sumOfPixelValues.AsQueryable().Average();

            // Заменяем каждое знанеие цвета пикселя на 1 или 0 в зависимости от того, больше оно среднего значения или меньше
            for (int i = 0; i < sumOfPixelValues.Length; i++)
            {
                if (sumOfPixelValues[i] >= averageSumOfPixelValues) sumOfPixelValues[i] = 1;
                else sumOfPixelValues[i] = 0;
            }
            return sumOfPixelValues;
        }
    }
}
