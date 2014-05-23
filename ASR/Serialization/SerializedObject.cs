using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASR.Serialization
{
    [Serializable]
    public class SerializedObject
    {
        public Type OriginalType { get; set; }
        public object SerializedContent { get; set; }

        [NonSerialized]
        private object DeserializedObject;
        private Func<object, object> DeserializeMethod;

        public SerializedObject(Func<object, object> d)
        {
            DeserializeMethod = d;
        }

        public object Deserialize()
        {
            return DeserializedObject ?? (DeserializedObject = DeserializeMethod(SerializedContent));
        }

        
    }
}
