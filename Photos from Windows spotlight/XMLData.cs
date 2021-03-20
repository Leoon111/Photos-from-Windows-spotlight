﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Photos_from_Windows_spotlight
{
    class XMLData
    {
        /// <summary>
        /// чтение данных из файла XML
        /// </summary>
        public List<PhotoData> Read()
        {
            var photoData = new List<PhotoData>();
            try
            {
                // Создание XPath документа.
                var document = new XPathDocument(@"../../data.xml");
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


            return photoData;
        }

        /// <summary>
        /// запись данных в файл XML
        /// </summary>
        public void Write()
        {
            // Создание XPath документа.
            var document = new XPathDocument(@"../../data.xml");

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
            var xmlWriter = new XmlTextWriter("../../data.xml", null)
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
    }
}