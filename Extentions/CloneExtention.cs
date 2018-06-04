using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Project.Extentions
{
    public static class CloneExtention
    {
        public static T Clone<T> (T source)
        {
            using (Stream obj = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(obj, source);
                obj.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(obj);
            }
        }
    }
}
