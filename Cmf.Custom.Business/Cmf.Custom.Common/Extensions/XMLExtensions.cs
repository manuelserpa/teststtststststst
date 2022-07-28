using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Cmf.Custom.AMSOsram.Common.Extensions
{
    public static class XMLExtensions
    {
        /// <summary>
        /// To xml.
        /// </summary>
        /// <typeparam name="T">Class.</typeparam>
        /// <param name="instance">Instance.</param>
        /// <returns>Xml.</returns>
        public static string ToXml<T>(this T instance) where T : class
        {
            // serialize the instance
            DataContractSerializer serializer = new DataContractSerializer(instance.GetType());

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    serializer.WriteObject(writer, instance);

                    return sw.ToString();
                }
            }
        }

        /// <summary>
        /// From Xml.
        /// </summary>
        /// <typeparam name="T">Class.</typeparam>
        /// <param name="instance">Instance.</param>
        /// <returns>Class object.</returns>
        public static T FromXml<T>(this string xml) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T deserialized = default(T);
            using (var stream = new StringReader(xml))
            {
                deserialized = serializer.Deserialize(stream) as T;
            }
            return deserialized;
        }
    }
}
