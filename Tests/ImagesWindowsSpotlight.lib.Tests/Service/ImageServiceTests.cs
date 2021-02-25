using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImagesWindowsSpotlight.lib.Service.Tests
{
    [TestClass()]
    public class ImageServiceTests
    {
        [TestMethod()]
        public void GetPerceptualHashOfImage_comparing_test_picture_with_its_perceptual_hash()
        {
            #region Arrange

            var imgJpg = Environment.CurrentDirectory
                .Replace(@"bin\Debug\netcoreapp3.1", @"Data\GetPerceptualHashOfImage.jpg");

            #endregion

            #region Act

            var actual_result_1 = new ImageService().GetPerceptualHashOfImage(imgJpg);
            string str = "";
            foreach (var b in actual_result_1)
            {
                str += b.ToString();
            }

            #endregion

            #region Assert

            Assert.AreEqual("1111001011110000111100001111000011110000111100001111001011100010", str);

            #endregion
        }
    }
}