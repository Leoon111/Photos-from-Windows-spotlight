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

        private Configuration _configurationDate;

        /// <summary>
        /// Переменная для сообщения, что файл конфигурации был изменен.
        /// </summary>
        private bool _needUpdateDataFile = false;

        static XMLData()
        {
            // todo перенести в файл конфигурации
            _pathToFileConfiguration = @"data.xml";
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
                return ReadXml();
            }
            return null;
        }

        /// <summary>
        /// чтение данных из файла XML
        /// </summary>
        public Configuration ReadXml()
        {
            _configurationDate = null;

            if (File.Exists(_pathToFileConfiguration))
            {
                string serializedData = File.ReadAllText(_pathToFileConfiguration);
                if (serializedData == "")
                {
                    return null;
                }
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                var stringReader = new StringReader(serializedData);
                _configurationDate = (Configuration)xmlSerializer.Deserialize(stringReader);
            }
            return _configurationDate;
        }

        /// <summary>
        /// Добавляет в файл конфигурации данные. (читает из xml и заменяет в ней все с добавленными данными)
        /// </summary>
        /// <param name="newConfiguration">Новые данные</param>
        public void AddNewConfiguration(Configuration newConfiguration)
        {
            Configuration xmlConfiguration = null;
            // проверяем с помощью флага, нужно нам ли загружать новые данные из файла.
            //if (_needUpdateDataFile)
            //{
               xmlConfiguration = ReadXml();
            //}
            if (xmlConfiguration != null)
            {
                if (newConfiguration.PathFiles != "" &&
                    xmlConfiguration.PathFiles != newConfiguration.PathFiles)
                {
                    xmlConfiguration.PathFiles = newConfiguration.PathFiles;
                }
                if (newConfiguration.PhotoDataset != null && newConfiguration.PhotoDataset.Count > 0)
                {
                    xmlConfiguration.PhotoDataset.AddRange(newConfiguration.PhotoDataset);
                }
            }
            else
            {
                Write(newConfiguration);
            }
            _needUpdateDataFile = true;
            //return xmlConfiguration;
        }

        /// <summary>
        /// Сереализует xml в файл (проверить, предыдущую конфигурацию в классе, файл перезаписывается)
        /// </summary>
        /// <param name="myConfiguration">данный для записи</param>
        public void Write(Configuration myConfiguration)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, myConfiguration);

            // xml для теста в виде строки потока
            string xml = stringWriter.ToString();

            File.WriteAllText(_pathToFileConfiguration, xml);
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
