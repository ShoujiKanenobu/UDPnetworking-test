using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    class SerializationHandler
    {
        public static byte[] Serialize<T>(T data) where T : struct
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, data);
            return stream.ToArray();
        }

        public static T Deserialize<T>(byte[] array) where T : struct 
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(array);
            return (T)formatter.Deserialize(stream);
        }
    }
}
