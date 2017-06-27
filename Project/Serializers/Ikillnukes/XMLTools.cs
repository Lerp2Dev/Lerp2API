using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Lerp2API.Serializers.Ikillnukes
{
    /// <summary>
    /// Class XMLTools.
    /// </summary>
    public class XMLTools
    {
        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="indented">if set to <c>true</c> [indented].</param>
        /// <returns>System.String.</returns>
        public static string Serialize<T>(T value, bool indented = false)
        {
            if (value == null)
            {
                Console.WriteLine("XMLSerializer - The value passed is null!");
                return "";
            }

            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                string serializeXml = "";

                using (StringWriter stringWriter = new StringWriter())
                {

                    using (XmlWriter writer = XmlWriter.Create(stringWriter))
                    {
                        xmlserializer.Serialize(writer, value);
                        serializeXml = stringWriter.ToString();
                    }

                    if (indented)
                    {
                        serializeXml = Beautify(serializeXml);
                    }

                }

                return serializeXml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T Deserialize<T>(string value)
        {

            try
            {
                object returnvalue = new object();
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                TextReader reader = new StringReader(value);

                returnvalue = xmlserializer.Deserialize(reader);

                reader.Close();
                return (T)returnvalue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }

        }

        /// <summary>
        /// Serializes to file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="indented">if set to <c>true</c> [indented].</param>
        public static void SerializeToFile<T>(T value, string filePath, bool indented = false)
        {
            if (value == null)
            {
                Console.WriteLine("XMLSerializer - The value passed is null!");
            }
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                using (StreamWriter fileWriter = new StreamWriter(filePath))
                {
                    if (indented)
                    {
                        using (StringWriter stringWriter = new StringWriter())
                        {
                            using (XmlWriter writer = XmlWriter.Create(stringWriter))
                            {
                                xmlserializer.Serialize(writer, value);
                                fileWriter.WriteLine(Beautify(stringWriter.ToString()));
                            }
                        }
                    }
                    else
                    {
                        using (XmlWriter writer = XmlWriter.Create(fileWriter))
                        {
                            xmlserializer.Serialize(writer, value);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">The file path.</param>
        /// <returns>T.</returns>
        public static T DeserializeFromFile<T>(string filePath)
        {

            try
            {
                object returnvalue = new object();
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                using (TextReader reader = new StreamReader(filePath))
                {
                    returnvalue = xmlserializer.Deserialize(reader);
                }
                return (T)returnvalue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }

        }

        /// <summary>
        /// Beautifies the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        public static string Beautify(object obj)
        {
            XmlDocument doc = new XmlDocument();
            if (obj.GetType() == typeof(string))
            {
                if (!string.IsNullOrEmpty((string)obj))
                {
                    try
                    {
                        doc.LoadXml((string)obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("XMLIndenter - Wrong string format! [" + ex.Message + "]");
                        return "";
                    }
                }
                else
                {
                    Console.WriteLine("XMLIndenter - String is null!");
                    return "";
                }
            }
            else if (obj.GetType() == typeof(XmlDocument))
            {
                doc = (XmlDocument)obj;
            }
            else
            {
                Console.WriteLine("XMLIndenter - Not supported type!");
                return "";
            }
            MemoryStream w = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(w, Encoding.Unicode);
            writer.Formatting = Formatting.Indented;
            doc.WriteContentTo(writer);

            writer.Flush();
            w.Seek(0L, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(w);
            return reader.ReadToEnd();
        }


        // Convert an object to a byte array
        /// <summary>
        /// Objects to byte array.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
        // Convert a byte array to an Object
        /// <summary>
        /// Bytes the array to object.
        /// </summary>
        /// <param name="arrBytes">The arr bytes.</param>
        /// <returns>System.Object.</returns>
        public static object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = binForm.Deserialize(memStream);
            return obj;
        }

    }
}
