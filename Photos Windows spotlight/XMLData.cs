using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Photos_Windows_spotlight
{
    class XMLData
    {

        /// <summary>
        /// путь к файлу конфигурации
        /// </summary>
        private string _pathToFileConfiguration;

        /// <summary>
        /// путь к папке с сохранненными фото.
        /// </summary>
        private string _pathSaveImage;

        public XMLData()
        {
            // todo перенести в файл конфигурации
            _pathToFileConfiguration = @"../../data.xml";
        }

        public string GetPathSaveImages()
        {

            return null;
        }

        private Configuration GetXmlConfigurations()
        {
            if (IsExistFileConfiguration())
            {
                return Read();
            }
            return null;
        }

        /// <summary>
        /// чтение данных из файла XML
        /// </summary>
        public Configuration Read()
        {
            var photoData = new List<PhotoData>();
            string serializedData = "";
            try
            {
                if (File.Exists(_pathToFileConfiguration))
                {
                    serializedData = File.ReadAllText(_pathToFileConfiguration);
                }

                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                var stringReader = new StringReader(serializedData);
                Configuration collection = (Configuration)xmlSerializer.Deserialize(stringReader);
            }

            catch (Exception)
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// запись данных в файл XML
        /// </summary>
        async public void Write(Configuration myConfiguration)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, myConfiguration);

            // xml для теста в виде строки потока
            string xml = stringWriter.ToString();

        }

        /// <summary>
        /// Создание файла с данными, если таковой не имеется.
        /// </summary>
        public void CreateNewDateFile()
        {
            var xmlWriter = new XmlTextWriter(_pathToFileConfiguration, null)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1,
                QuoteChar = '\"'
            };

            // формат данных в xml
            ////<Photos>
            ////    <PhotoData>
            ////        <Name> </Name>
            ////        <Size> </Size>
            ////        <FileDate> </FileDate>
            ////        <PreviousName> </PreviousName>
            ////    </PhotoData>
            ////</Photos

            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("Photos");

            // данные для теста
            {
                xmlWriter.WriteStartElement("PhotoData");
                xmlWriter.WriteElementString("Name", "123");
                xmlWriter.WriteElementString("Size", "45");
                xmlWriter.WriteElementString("FileDate", "12.12.12");
                xmlWriter.WriteElementString("PreviousName", "001100");
            }

            xmlWriter.Close();

        }

        /// <summary>
        /// Проверяем наличие файла конфигурации в папке с файлом
        /// </summary>
        /// <returns></returns>
        public static bool IsExistFileConfiguration()
        {
            return File.Exists(@"../../data.xml");
        }
    }
}
