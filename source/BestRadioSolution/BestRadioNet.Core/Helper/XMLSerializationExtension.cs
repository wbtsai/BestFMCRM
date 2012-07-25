using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace BestRadioNet.Core.Helper
{
    public static class XMLSerializationExtension
    {
        /// <summary>
        /// 由XML File 轉Class
        /// 這是由檔案讀取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFileName">C:/xml/aa.xml</param>
        /// <returns></returns>
        public static T XMLFileToClass<T>(this string xmlFileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                return (T)serializer.Deserialize(sr);
            }

            
        }

        public static T XMLStringToClass<T>(this string xmlString)
        {
            StringReader read = new StringReader(xmlString);

            XmlReader reader = new XmlTextReader(read);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        /// 由Class轉XML檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="xmlFilePath"></param>
        public static void ClassToXmlFile<T>(this T obj, string xmlFilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            TextWriter tw = new StreamWriter(xmlFilePath);

            serializer.Serialize(tw, obj);

            tw.Close();

        }

        public static void ClassToXmlFileWithoutNS<T>(this T obj, string xmlFilePath)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings writerSettings = new XmlWriterSettings();

            writerSettings.OmitXmlDeclaration = true;

            StringWriter stringWriter = new StringWriter(); 
            
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
            {
                serializer.Serialize(xmlWriter, obj,ns);

                using(TextWriter tw = new StreamWriter(xmlFilePath))
                {
                    tw.Write(stringWriter.ToString());
                }
               
            }
        }

        /// <summary>
        /// 由Class 轉成 XML Document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static string ClassToXmlDocument<T>(this T obj)
        {
            StringWriter stringWriter = new StringWriter();

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            serializer.Serialize(stringWriter, obj); //(obj, new XmlNodeWriter(doc));

            return stringWriter.ToString();



        }

        
    }
}
