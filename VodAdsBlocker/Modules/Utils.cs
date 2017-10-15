using System.IO;
using System.Xml.Serialization;

namespace VodAdsBlocker.Modules
{
    public static class Utils
    {
        public static T DeserializeFromFile<T>(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        public static void SerializeToFile<T>(this T o, string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.OpenOrCreate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(stream, o);
            }
        }
    }
}
