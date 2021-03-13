using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using LeaderAnalytics.Caching;

namespace LeaderAnalytics.Core.Serialization.XML
{
    public static class SerializationHelper<T> where T : new()
    {
        private static ICache<XmlSerializer> cache = new Cache<XmlSerializer>(null);

        private static ICache<XmlRootAttribute> rootAttributeCache = new Cache<XmlRootAttribute>(null);

        private static XmlSerializer GetCachedXmlSerializer(Type type, XmlRootAttribute root)
        {
            var key = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", type, root.ElementName);
            XmlSerializer xs = cache.Get(key);

            if (xs == null)
            {
                xs = new XmlSerializer(type, root);
                cache.Set(key, xs);
            }
            return xs;
        }

        private static XmlRootAttribute GetCachedRootAttribute(string xmlRoot)
        {
            XmlRootAttribute rootAttribute = rootAttributeCache.Get(xmlRoot);

            if (rootAttribute == null)
            {
                rootAttribute = new XmlRootAttribute(xmlRoot);
                rootAttributeCache.Set(xmlRoot, rootAttribute);
            }
            return rootAttribute;
        }

        public static string Serialize(T obj, XmlSerializerNamespaces nameSpaces)
        {
            string xml = null;

            if (obj == null)
                return xml;

            using (StringWriter sw = new StringWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(sw, obj, nameSpaces);
                xml = sw.ToString();
            }
            return xml;
        }

        public static string Serialize(T obj)
        {
            return Serialize(obj, null);
        }

        public static T DeSerialize(string xml, XmlRootAttribute xmlRoot)
        {
            T obj = default(T);

            if (string.IsNullOrWhiteSpace(xml))
                return obj;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                obj = (T)GetCachedXmlSerializer(typeof(T), xmlRoot).Deserialize(memoryStream);
                
            return obj;
        }

        public static T DeSerialize(Stream stream, XmlRootAttribute xmlRoot)
        {
            T obj = default(T);

            if (stream == null)
                return obj;
            
            obj = (T)GetCachedXmlSerializer(typeof(T), xmlRoot).Deserialize(stream);
            return obj;
        }

        public static T DeSerialize(string xml, string xmlRoot)
        {
            return DeSerialize(xml, new XmlRootAttribute(xmlRoot));
        }

        public static T DeSerialize(Stream stream, string xmlRoot)
        {
            return DeSerialize(stream, GetCachedRootAttribute(xmlRoot));
        }

        public static T DeSerialize(string xml)
        {
            return DeSerialize(xml, string.Empty);
        }

        public static T Clone(T original)
        {
            return DeSerialize(Serialize(original));
        }
    }
}
