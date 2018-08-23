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
        private static string _pathToFileConfiguration;

        /// <summary>
        /// путь к папке с сохранненными фото.
        /// </summary>
        private string _pathSaveImage;

        static XMLData()
        {
            // todo перенести в файл конфигурации
            _pathToFileConfiguration = @"../../data.xml";
        }

        public string GetPathSaveImages()
        {

            return null;
        }

        /// <summary>
        /// Получение данных из файла конфигурации, ошибки не отлавливаются, выкидываются наверх.
        /// </summary>
        /// <returns></returns>
        public Configuration GetXmlConfigurations()
        {
            // todo оченить, нужна ли тут проверка, если ошибки специально выкидываем наерх?
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
            Configuration collectionDate = null;

            if (File.Exists(_pathToFileConfiguration))
            {
                string serializedData = File.ReadAllText(_pathToFileConfiguration);
                if (serializedData == "")
                {
                    return null;
                }
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                var stringReader = new StringReader(serializedData);
                collectionDate = (Configuration)xmlSerializer.Deserialize(stringReader);
            }
            return collectionDate;
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
            return File.Exists(_pathToFileConfiguration);
        }

        /// <summary>
        /// Создаем файл конфигурации
        /// </summary>
        /// <returns></returns>
        public static bool CreateXMLFile()
        {
            File.Create(_pathToFileConfiguration);
            return true;
        }
    }
}
