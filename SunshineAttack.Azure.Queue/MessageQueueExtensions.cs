using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SunshineAttack.Azure.TypedQueueMessage
{
    public static class MessageQueueExtensions
    {
        public static T Deserialize<T>(this CloudQueueMessage message) where T : class
        {
            return message == null ? null : message.AsBytes.Deserialize<T>();
        }

        private static T Deserialize<T>(this byte[] serializedObject)
        {
            return Deserialize<T>(serializedObject, new UTF8Encoding());
        }

        private static T Deserialize<T>(this byte[] serializedObject, Encoding encoding)
        {
            var xmlSer = new XmlSerializer(typeof(T));
            return (T)xmlSer.Deserialize(new StringReader(serializedObject.DeSerialize(encoding)));
        }

        private static byte[] Serialize(this string stringToCompress, Encoding encoding)
        {
            byte[] stringAsBytes = encoding.GetBytes(stringToCompress);
            using (var memoryStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memoryStream,
                                                      CompressionMode.Compress))
                {
                    zipStream.Write(stringAsBytes, 0, stringAsBytes.Length);
                    zipStream.Close();
                    return (memoryStream.ToArray());
                }
            }
        }

        private static byte[] Serialize(this object objectToSerialize, Encoding encoding)
        {
            var xmlSerializer = new XmlSerializer(objectToSerialize.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, objectToSerialize);
                return stringWriter.ToString().Serialize(encoding);
            }
        }

        public static byte[] Serialize(this object objectToSerialize)
        {
            return Serialize(objectToSerialize, new UTF8Encoding());
        }

        private static string DeSerialize(this byte[] compressedString, Encoding encoding)
        {
            const int bufferSize = 1024;
            using (var memoryStream = new MemoryStream(compressedString))
            {
                using (var zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    // Memory stream for storing the decompressed bytes
                    using (var outStream = new MemoryStream())
                    {
                        var buffer = new byte[bufferSize];
                        int totalBytes = 0;
                        int readBytes;
                        while ((readBytes = zipStream.Read(buffer, 0, bufferSize)) > 0)
                        {
                            outStream.Write(buffer, 0, readBytes);
                            totalBytes += readBytes;
                        }
                        return encoding.GetString(
                            outStream.GetBuffer(), 0, totalBytes);
                    }
                }
            }
        }
    }
}