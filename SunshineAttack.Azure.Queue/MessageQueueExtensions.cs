using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SunshineAttack.Azure.TypedQueueMessage
{
    public static class MessageQueueExtensions
    {
        public static T DecompressToObject<T>(this CloudQueueMessage message) where T : class
        {
            return message == null ? null : message.AsBytes.DecompressToObject<T>();
        }

        public static T DecompressToObject<T>(this byte[] compressedObject)
        {
            return DecompressToObject<T>(compressedObject, new UTF8Encoding());
        }

        private static T DecompressToObject<T>(this byte[] compressedObject, Encoding encoding)
        {
            var xmlSer = new XmlSerializer(typeof(T));
            return (T)xmlSer.Deserialize(new StringReader(compressedObject.DecompressToString(encoding)));
        }

        private static string DecompressToString(this byte[] compressedString, Encoding encoding)
        {
            const int bufferSize = 1024;
            using (var memoryStream = new MemoryStream(compressedString))
            {
                using (var zipStream = new GZipStream(memoryStream,
                    CompressionMode.Decompress))
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