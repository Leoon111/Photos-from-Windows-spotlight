using System;
using ImagesWindowsSpotlight.lib.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImagesWindowsSpotlight.lib.Tests.Service
{
    [TestClass]
    public class ImageCheckTests
    {
        [TestMethod]
        public void HasJpegHeader_jpg_start_marker_check()
        {
            #region Arrange

            // получаем адрес картинки в данных приложения путем получения адреса запускаемого файла и замены части пути на путь к тестовому изображению
            //@"E:\MyApps\Мои приложения\Фото с заставки рабочего стола виндовс\Photos-from-Windows-spotlight\Tests\ImagesWindowsSpotlight.lib.Tests\Data\A7RNHSMC65E.jpg";
            var imgJpg = Environment.CurrentDirectory
                .Replace(@"bin\Debug\netcoreapp3.1", @"Data\A7RNHSMC65E.jpg");
            var imgPng = Environment.CurrentDirectory
                .Replace(@"bin\Debug\netcoreapp3.1", @"Data\net.png");

            #endregion

            #region Act

            var actual_result_1 = ImageCheck.HasJpegHeader(imgJpg);
            var actual_result_2 = ImageCheck.HasJpegHeader(imgPng);

            #endregion

            #region Assert

            Assert.AreEqual(true, actual_result_1);
            Assert.AreEqual(false, actual_result_2);

            #endregion
        }
    }
}
