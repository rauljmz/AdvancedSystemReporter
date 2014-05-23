using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASR.Serialization
{
    public static class SerializatorsFactory
    {
        private static Dictionary<Type, object> serializators;
        static SerializatorsFactory()
        {
            serializators = new Dictionary<Type, object>();
        }

        public static void RegisterSerializator(Type t, ISerializator serializator)
        {
            if (serializators.ContainsKey(t))
            {
                serializators[t] = serializator;
            }
            else
            {
                serializators.Add(t, serializator);
            }
        }
        public static object Serialize(object o)
        {
            var type = o.GetType();
            if (type.IsSerializable) return o;
            var s = GetSerializator(type);
            return s.Serialize(o);
        }

        private static ISerializator GetSerializator(Type t)
        {
            return serializators[t] as ISerializator;
        }
    }
}
