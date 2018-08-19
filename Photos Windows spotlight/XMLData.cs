using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
            try
            {
                // Создание XPath документа.
                var document = new XPathDocument(_pathToFileConfiguration);
                XPathNavigator navigator = document.CreateNavigator();

                // Прямой запрос XPath.
                XPathNodeIterator iterator1 = navigator.Select("Photos/PhotoData");
                while (iterator1.MoveNext())
                {
                    Console.WriteLine(iterator1.Current);
                }

                // Скомпилированный запрос XPath
                //XPathExpression expr = navigator.Compile("ListOfBooks/Book[2]/Price");
                //XPathNodeIterator iterator2 = navigator.Select(expr);
                //while (iterator2.MoveNext())
                //{
                //    Console.WriteLine(iterator2.Current);
                //}
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
        public void Write()
        {
            // Создание XPath документа.
            var document = new XPathDocument(_pathToFileConfiguration);

            // Единственное назначение XPathDocument - создание навигатора.
            XPathNavigator navigator = document.CreateNavigator();

            // При создании навигатора при помощи XPathDocument возможно выполнять только чтение.
            Console.WriteLine("Навигатор создан только для чтения. Свойство CanEdit = {0}.", navigator.CanEdit);

            // Используя XmlDocument навигатор можно использовать и для редактирования.
            var xmldoc = new XmlDocument();
            xmldoc.Load("books.xml");

            navigator = xmldoc.CreateNavigator();
            Console.WriteLine("Навигатор получил возможность редактирования. Свойство CanEdit = {0}.", navigator.CanEdit);

            // Теперь можно попробовать что-то записать в XML-документ.
            // Выполняем навигацию к узлу Book.
            navigator.MoveToChild("ListOfBooks", "");
            navigator.MoveToChild("Book", "");

            // Проводим вставку значений.
            navigator.InsertBefore("<InsertBefore>insert_before</InsertBefore>");
            navigator.InsertAfter("<InsertAfter>insert_after</InsertAfter>");
            navigator.AppendChild("<AppendChild>append_child</AppendChild>");

            navigator.MoveToNext("Book", "");

            navigator.InsertBefore("<InsertBefore>1111111111</InsertBefore>");
            navigator.InsertAfter("<InsertAfter>2222222222</InsertAfter>");
            navigator.AppendChild("<AppendChild>3333333333</AppendChild>");

            // Сохраняем изменения.
            xmldoc.Save("books.xml");

            // Delay.
            Console.ReadKey();


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
