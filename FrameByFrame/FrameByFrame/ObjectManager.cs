using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameByFrame
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The object manager.
    /// </summary>
    public static class ObjectManager
    {
        #region Public Methods and Operators

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public static T Clone<T>(this T from)
        {
            return CopyObject(from);
        }

        /// <summary>
        /// The copy object.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public static T CopyObject<T>(T from)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();

            bf.Serialize(ms, from);
            ms.Position = 0;
            var t = (T)bf.Deserialize(ms);
            return t;
        }

        /// <summary>
        /// The from binary.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public static T FromBinary<T>(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var t = (T)formatter.Deserialize(stream);
            stream.Close();
            return t;
        }

        /// <summary>
        /// The to binary.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void ToBinary<T>(string fileName, T t)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, t);
            stream.Close();
        }

        private static string ToXml<T>(T t)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringWriter sww = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sww);
            ser.Serialize(writer, t);
            return sww.ToString();
        }

        public static void ToXmlFile<T>(string fileName, T t)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.WriteAllText(fileName, ToXml<T>(t));
        }

        public static T FromXml<T>(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(fileName);
            //reader.ReadToEnd();
            object o = serializer.Deserialize(reader);
            reader.Close();
            return (T)o;
        }

        //public static T FromJsonFile<T>(string fileName)
        //{
        //    return FromJson<T>(File.ReadAllText(fileName));
        //}
        //public static T FromJson<T>(string jsonData)
        //{
        //    if (jsonData == null)
        //    {
        //        throw new ArgumentNullException("jsonData");
        //    }

        //    Type type = typeof(T);

        //    try
        //    {
        //        DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
        //        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData)))
        //        {
        //            T data = (T)serializer.ReadObject(stream);
        //            return data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = string.Format("JSON Deserialization failed for type {0}.", type.FullName);
        //        throw new SerializationException(msg, ex);
        //    }
        //}

        //public static void ToJsonFile(string fileName, object data)
        //{
        //    if (File.Exists(fileName))
        //        File.Delete(fileName);

        //    File.WriteAllText(fileName, ToJson(data));
        //}

        //public static string ToJson(object data)
        //{
        //    if (data == null)
        //    {
        //        throw new ArgumentNullException("data");
        //    }

        //    Type type = data.GetType();

        //    try
        //    {
        //        DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
        //        using (var stream = new MemoryStream())
        //        {
        //            serializer.WriteObject(stream, data);
        //            string jsonData = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
        //            return jsonData;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = string.Format("JSON Serialization failed for type {0}.", type.FullName);
        //        throw new SerializationException(msg, ex);
        //    }
        //    finally
        //    {
        //    }
        //}

        #endregion
    }
}
