using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASR.Serialization
{

    public interface ISerializator
    {
        SerializedObject Serialize(object thingToSerialize);
        Type Type { get; }
    }
}
