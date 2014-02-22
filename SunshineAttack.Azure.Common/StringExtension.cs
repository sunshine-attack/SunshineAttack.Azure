using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;

namespace SunshineAttack.Azure.Common
{
    public static class StringExtensions
    {
        public static byte[] Compress(this string stringToCompress, Encoding encoding)
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

        public static byte[] Compress(this string stringToCompress)
        {
            return Compress(stringToCompress, new UTF8Encoding());
        }

        public static byte[] Compress(this object objectToCompress, Encoding encoding)
        {
            var xmlSerializer = new XmlSerializer(objectToCompress.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, objectToCompress);
                return stringWriter.ToString().Compress(encoding);
            }
        }

        public static byte[] Compress(this object objectToCompress)
        {
            return Compress(objectToCompress, new UTF8Encoding());
        }

        public static string DecompressToString(this byte[] compressedString, Encoding encoding)
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
                        var totalBytes = 0;
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


        public static string DecompressToString(this byte[] compressedString)
        {
            return DecompressToString(compressedString, new UTF8Encoding());
        }


        public static T DecompressToObject<T>(this byte[] compressedObject, Encoding encoding)
        {
            var xmlSer = new XmlSerializer(typeof (T));
            return (T) xmlSer.Deserialize(new StringReader(compressedObject.DecompressToString(encoding)));
        }


     
    }
}